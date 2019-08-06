using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using GameProto;
using Google.Protobuf;

namespace Core.Network {

    public delegate void NetworkStateChangedCallback( NetworkProtoClient.NetworkState newState );

    public class NetworkProtoClient : MonoBehaviour {

        private class ConnectionContext {
            public string IP { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
        }

        public enum NetworkState {
            CONNECTING,
            CONNECTION_RETRY,
            CONNECTED,
            DELAY,
            CLOSED
        }

        public class SessionContext {
            //public Package_Head Head;
            public Action<byte[]> ResponseCallback;
            public float Time;
        }

        private NetworkTcpClient client;
        public event NetworkStateChangedCallback NetworkStateChanged;

        public long Session {
            get;
            set;
        }
        private NetworkState networkState = NetworkState.CLOSED;
        private float connectTime;
        private int retryTimes;
        private ConnectionContext connection;

        public NetworkState CurrentState { get { return networkState; } }
        private Dictionary<long, SessionContext> sessionDict;

        public delegate void ServerRequest( byte[] request );

        private Dictionary<string, ServerRequest> s2cRequest = new Dictionary<string, ServerRequest>();

        public void RegisterRequestCallback<T>( ServerRequest request ) {
            string tag = NetworkUtils.Instance.ProtoTags[typeof( T )];
            if( !s2cRequest.ContainsKey( tag ) )
                s2cRequest.Add( tag, request );
        }

        public void UnregisterRequestCallback<T>() {
            string tag = NetworkUtils.Instance.ProtoTags[typeof( T )];
            s2cRequest.Remove( tag );
        }

        public NetworkProtoClient() {
            sessionDict = new Dictionary<long, SessionContext>();
        }

        public long DoRequest<T>( byte[] data, Action<byte[]> response ) {
            Session = Session + 1;
            if( Session > 65535 ) {
                Session = 0;
            }
            Package_Head pkg = new Package_Head {
                Type = typeof( T ).ToString(),
                Session = Session
            };

            if( response != null ) {
                sessionDict.Add( Session, new SessionContext() {
                    ResponseCallback = response
                } );
            }

            byte[] head = pkg.ToByteArray();
            int headLength = head.Length;
            int length = data.Length + headLength + 2;

            byte[] buffer = new byte[length];
            buffer[0] = (byte)headLength;
            buffer[1] = (byte)data.Length;

            int index = 0;
            for( int i = 2; i < headLength + 2; i++ ) {
                buffer[i] = head[index];
                index++;
            }
            index = 0;
            for( int i = 2 + headLength; i < length; i++ ) {
                buffer[i] = data[index];
                index++;
            }
            client.Send( buffer, length );
            return Session;
        }

        void SetState( NetworkState state ) {
            if( networkState == state && state != NetworkState.CONNECTION_RETRY ) {
                return;
            }

            NetworkStateChanged?.Invoke( state );

            networkState = state;
            switch( state ) {
                case NetworkState.CONNECTING:
                    connectTime = 0;
                    retryTimes = 0;
                    break;
                case NetworkState.CONNECTION_RETRY:
                    if( retryTimes == 10 ) {
                        SetState( NetworkState.CLOSED );
                        return;
                    }
                    RetryConnect();
                    retryTimes += 1;
                    break;
                case NetworkState.CLOSED:
                    retryTimes = 0;
                    client.Close();
                    break;
                case NetworkState.CONNECTED:
                    retryTimes = 0;
                    connectTime = 0;
                    break;
            }
        }

        void Update() {
            if( client == null )
                return;
            var datas = client.Dispatch();
            while( datas != null ) {

                int headLength = (int)datas[0];
                int dataLength = (int)datas[1];

                Debug.Assert( headLength + dataLength + 2 == datas.Length );

                byte[] head = new byte[headLength];
                byte[] data = new byte[dataLength];

                int index = 0;
                for( int i = 2; i < headLength + 2; i++ ) {
                    head[index] = datas[i];
                    index++;
                }
                index = 0;
                for( int i = headLength + 2; i < datas.Length; i++ ) {
                    data[index] = datas[i];
                    index++;
                }

                Package_Head pkg = Utils.ParseByte<Package_Head>( head );

                var type = pkg.Type;
                var session = pkg.Session;
                if( session < 0 ) {
                    ServerRequest serverReq;
                    if( s2cRequest.TryGetValue( type, out serverReq ) ) {
                        serverReq( data );
                    }
                }
                else {
                    SessionContext context;
                    if( sessionDict.TryGetValue( session, out context ) ) {
                        sessionDict.Remove( session );
                        context.ResponseCallback( data );
                    }
                }
                datas = client.Dispatch();
            }

            var deltaTime = Time.deltaTime;
            deltaTime = deltaTime > .1f ? .1f : deltaTime;
            if( CurrentState == NetworkState.CONNECTING || CurrentState == NetworkState.CONNECTION_RETRY ) {
                connectTime += Time.deltaTime;
                if( connectTime >= 2 ) {
                    SetState( NetworkState.CONNECTION_RETRY );
                    connectTime = 0;
                }
            }
            else if( CurrentState == NetworkState.CONNECTED ) {
                foreach( var rpc in sessionDict.Values ) {
                    rpc.Time += deltaTime;
                    if( rpc.Time > 10 ) {
                        sessionDict.Clear();
                        SetState( NetworkState.CONNECTION_RETRY );
                        break;
                    }
                }
            }
        }

        public void RetryConnect() {
            client.Close();
            if( CurrentState == NetworkState.CONNECTED ) {
                SetState( NetworkState.CLOSED );
            }
            if( string.IsNullOrEmpty( connection.Host ) ) {
                client.Connnect( connection.IP, connection.Port );
            }
            else {
                client.ConnectDomain( connection.Host, connection.Port );
            }
        }

        public void Connect( string host, int port ) {
            PreConnected();
            connection = new ConnectionContext() {
                Port = port
            };
            if( Regex.IsMatch( host, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$" ) ) {
                connection.IP = host;
            }
            else {
                connection.Host = host;
            }

            RetryConnect();
        }

        private void PreConnected() {
            if( client != null ) {
                client.Destroy();
                client = null;
            }

            client = new NetworkTcpClient();
            client.StateChanged += ( NetworkTcpClient sender, bool connected ) => {
                if( connected ) {
                    SetState( NetworkState.CONNECTED );
                }
                else {
                    SetState( NetworkState.CONNECTION_RETRY );
                }
            };

            sessionDict.Clear();
            SetState( NetworkState.CONNECTING );
        }
    }
}