using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Core.Network;
using LitJson;
using GameProto;

namespace Game.Network {

    public class NetworkManager : Singleton<NetworkManager> {
        public enum NetworkManagerState {
            CLOSED,
            CONNECTED,
            CONNECTING,
            CONNECTION_RETRY,
        }

        public enum LoginState {
            NONE,
            LOGINED,
            DISCONNECTED,
        }
        /// <summary>
        /// 长连接状态切换事件
        /// </summary>
        public event Action<Login, bool> OnLoginFinish;

        private NetworkManagerState managerState = NetworkManagerState.CLOSED;
        public NetworkManagerState CurrentNetworkState { get { return managerState; } }

        public LoginState CurrentLoginState {
            get;
            private set;
        }

        private class TCPRequest : IComparable<TCPRequest> {
            public long Session;
            public long RSession;
            public Type TemplateType;
            public byte[] Request;
            public Action<byte[]> Callback;

            public float ReqTime;

            public int CompareTo( TCPRequest other ) {
                return this.Session.CompareTo( other.Session );
            }
        }

        private float serverTime = -1f;
        private float loginTime = -1f;
        /// <summary>
        /// 获取当前时间
        /// </summary>
        public float CurrentTime {
            get {
                if( serverTime == -1 )
                    return 0;
                else
                    return serverTime + ( Time.time - loginTime );
            }
        }

        //最大请求数量上限
        public const int MaxRequestCount = 30;
        private Dictionary<long, TCPRequest> requests = new Dictionary<long, TCPRequest>();
        private List<TCPRequest> reconnectRequests = new List<TCPRequest>();
        private TCPRequest currentReconnectRequest = null;

        public static long ReconnectSession = 0;
        public static string TCPHostUrl = null;
        public static int TCPPort = -1;
        public static JsonData LoginToken = null;

        private NetworkProtoClient _tcpClient = null;
        private NetworkProtoClient tcpClient {
            get {
                if( _tcpClient == null ) {
                    _tcpClient = transform.GetOrAddComponent<NetworkProtoClient>();
                    _tcpClient.NetworkStateChanged += TCPNetworkStateChanged;
                }
                return _tcpClient;
            }
        }

        public Action<NetworkManagerState> OnStateChange;

        private bool isConnectionRetry = false;

        void SetState( NetworkManagerState state ) {
            if( managerState == state ) {
                return;
            }

            switch( state ) {
                case NetworkManagerState.CLOSED:
                    ShowErrorMessage( "链接已断开", false );
                    break;
                case NetworkManagerState.CONNECTION_RETRY:
                    if( CurrentLoginState == LoginState.LOGINED ) {
                        isConnectionRetry = true;
                        //弹板
                    }
                    break;
                case NetworkManagerState.CONNECTED:
                    if( isConnectionRetry ) {
                        isConnectionRetry = false;
                        Debug.Log( "Do reconnect TCPLogin" );
                        TcpLogin();
                    }
                    break;
                case NetworkManagerState.CONNECTING:
                    break;
            }

            if( managerState == NetworkManagerState.CONNECTED && state != NetworkManagerState.CONNECTED ) {
                CurrentLoginState = LoginState.DISCONNECTED;
            }

            managerState = state;
            if( OnStateChange != null ) {
                OnStateChange( state );
            }
        }

        private void TCPNetworkStateChanged( NetworkProtoClient.NetworkState newState ) {
            Debug.Log( "Network:TCP" + newState );
            switch( newState ) {
                case NetworkProtoClient.NetworkState.CONNECTING:
                    break;
                case NetworkProtoClient.NetworkState.CONNECTION_RETRY:
                    SetState( NetworkManagerState.CONNECTION_RETRY );
                    break;
                case NetworkProtoClient.NetworkState.CONNECTED:
                    SetState( NetworkManagerState.CONNECTED );
                    break;
                case NetworkProtoClient.NetworkState.DELAY:
                    break;
                case NetworkProtoClient.NetworkState.CLOSED:
                    SetState( NetworkManagerState.CLOSED );
                    Debug.Log( "Network:TCP Connection Closed" );
                    break;
                default:
                    break;
            }
        }

        private void TcpConnect() {
            if( TCPHostUrl == null || TCPPort == -1 ) {
                Debug.LogError( "LongConnection Error:Wrong Url or Port Number" );
                return;
            }
            tcpClient.Connect( TCPHostUrl, TCPPort );
        }

        /// <summary>
        /// 开启长连接
        /// </summary>
        public void StartConnection() {
            TcpConnect();
            SetState( NetworkManagerState.CONNECTING );
        }

        public void ResetConnection() {
            managerState = NetworkManagerState.CLOSED;
            tcpClient.RetryConnect();
            isConnectionRetry = false;
            SetState( NetworkManagerState.CONNECTING );
        }

        public NetworkProtoClient.NetworkState GetConnectionState() {
            return tcpClient.CurrentState;
        }

        public long DoRequest<T>( byte[] rpc, Action<byte[]> requestCallback ) {
            if( CurrentNetworkState != NetworkManagerState.CONNECTED && CurrentLoginState != LoginState.LOGINED ) {
                if( !isConnectionRetry ) {
                    ShowErrorMessage( "与服务器连接失败" );
                }
                Debug.LogError( "DoRequestError :  UnConnected" );
                return -1;
            }

            if( requests.Count >= MaxRequestCount ) {
                Debug.LogError( "DoRequestError :  Doreuqest Reach Limit Count" );
                return -2;
            }

            var req = new TCPRequest {
                Request = rpc,
                ReqTime = Time.time,
                TemplateType = typeof( T ),
            };

            if( requestCallback == null ) {
                tcpClient.DoRequest<T>( rpc, null );
                return 0;
            }

            req.Callback = ( byte[] response ) => {
                requests.Remove( req.Session );

                //没办法统一处理r值
                //因为返回的是字节数组，不知道什么数据
                requestCallback( response );
            };

            req.Session = tcpClient.DoRequest<T>( rpc, req.Callback );
            requests.Add( req.Session, req );
            return req.Session;
        }

        public void RegisterRequestCallback<T>( NetworkProtoClient.ServerRequest callback ) {
            tcpClient.RegisterRequestCallback<T>( callback );
        }

        public void UnregisterRequestCallback<T>() {
            tcpClient.UnregisterRequestCallback<T>();
        }

        private bool isTcpLogging;

        public void TcpLogin() {
            if( isTcpLogging )
                return;
            if( managerState != NetworkManagerState.CONNECTED ) {
                Debug.Log( "StateError" );
                return;
            }

            if( LoginToken == null ) {
                Debug.Log( "Token not Existed" );
                return;
            }
            string token = LoginToken["token"].ValueAsString();
            byte[] data = Utils.String2Byte( token );

            isTcpLogging = true;
            Debug.Log( "Do TcpLogin Request" );
            tcpClient.DoRequest<Login>( data, ( byte[] response ) => {
                isTcpLogging = false;

                Login login = Utils.ParseByte<Login>( response );

                if( managerState != NetworkManagerState.CONNECTED ) {
                    Debug.Log( "StateError" );
                    OnLoginFinish( login, false );
                    return;
                }

                if( login.R != 0 ) {
                    Debug.Log( "LoginError" );
                    OnLoginFinish( login, false );
                    return;
                }

                serverTime = login.Time / 100;
                loginTime = Time.time;

                CurrentLoginState = LoginState.LOGINED;

                if( OnLoginFinish != null ) {
                    OnLoginFinish( login, true );
                }
            } );
        }

        public NetworkManager() {
            CurrentLoginState = LoginState.NONE;
        }

        private static readonly float DEFAULT_HEARTBEAT_CD_TIME = 10;
        private float heartbeatCooldown = DEFAULT_HEARTBEAT_CD_TIME;

        void Update() {
            TimeOutDisconnect();
            if( managerState == NetworkManagerState.CONNECTED && CurrentLoginState == LoginState.LOGINED ) {
                if( currentReconnectRequest == null ) {
                    if( reconnectRequests.Count > 0 ) {
                        ReconnectSession += 1;
                        var req = reconnectRequests[0];
                        currentReconnectRequest = reconnectRequests[0];
                        reconnectRequests.RemoveAt( 0 );
                        currentReconnectRequest.RSession = ReconnectSession;

                        MethodInfo doRequestMethod = tcpClient.GetType().GetMethod( "DoRequest", BindingFlags.Public );
                        doRequestMethod = doRequestMethod.MakeGenericMethod( req.TemplateType );

                        Action<byte[]> callback = ( info ) => {
                            Debug.Log( "---Reconnect Request Recovery---" );
                            req.Callback( info );
                        };
                        var arguments = new object[]
                        {
                            req.Request,
                            callback,
                        };

                        doRequestMethod.Invoke( tcpClient, arguments );
                    }
                }

                heartbeatCooldown -= Time.deltaTime;
                if( heartbeatCooldown < 0 ) {
                    heartbeatCooldown = DEFAULT_HEARTBEAT_CD_TIME;
                    DoRequest<Heartbeat>( new byte[1], null );
                }
            }
        }


        public void ShowErrorMessage( string msg, bool clearRequest = true, bool needRelogin = true ) {
            CurrentLoginState = LoginState.DISCONNECTED;
            if( clearRequest ) {
                ResetRequest();
                ResetReconnectRequest();
            }
            //错误弹板
        }

        private void TimeOutDisconnect() {
            bool clear = false;
            foreach( var req in requests ) {
                if( Time.time - req.Value.ReqTime > 5f ) {
                    clear = true;
                    ShowErrorMessage( "回包超时！请重新登陆\n" + req.Value.Request.GetType().FullName, false );
                    Debug.Log( "---ResponseTimeOut:" + req.Value.Request.GetType().FullName + "---" );
                    //req.Value.Param.Dump();
                }
            }
            if( clear )
                requests.Clear();
        }

        private void ResetRequest() {
            requests.Clear();
        }

        private void ResetReconnectRequest() {
            this.currentReconnectRequest = null;
            this.reconnectRequests.Clear();
        }

        public new void OnDestroy() {
            base.OnDestroy();
        }

    }
}

