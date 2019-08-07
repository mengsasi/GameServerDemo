using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using GameProto;
using Google.Protobuf;
using System.IO;

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

        private Dictionary<long, SessionContext> sessionDict;
        public NetworkState CurrentState { get { return networkState; } }

        private static ProtoStream sendStream = new ProtoStream();
        private static readonly int MAX_PACK_LEN = ( 1 << 16 ) - 1;

        public delegate void ServerRequest( byte[] request );

        private Dictionary<string, ServerRequest> s2cRequest = new Dictionary<string, ServerRequest>();

        public void RegisterRequestCallback<T>( ServerRequest request ) {
            string tag = Utils.GetProtpType<T>();
            if( !s2cRequest.ContainsKey( tag ) )
                s2cRequest.Add( tag, request );
        }

        public void UnregisterRequestCallback<T>() {
            string tag = Utils.GetProtpType<T>();
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

            if( length > MAX_PACK_LEN ) {
                Debug.LogError( "data.Length > " + MAX_PACK_LEN + " => " + length );
                return Session;
            }

            sendStream.Seek( 0, SeekOrigin.Begin );
            sendStream.WriteByte( (byte)headLength );
            sendStream.WriteByte( (byte)data.Length );
            sendStream.Write( head, 0, head.Length );
            sendStream.Write( data, 0, data.Length );

            client.Send( sendStream.Buffer, sendStream.Position );
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

                byte[] head = Utils.CopyBytes( datas, 2, headLength + 2 );
                byte[] data = Utils.CopyBytes( datas, headLength + 2, datas.Length );

                Package_Head pkg = Utils.ParseByte<Package_Head>( head );

                var type = pkg.Type;
                var session = pkg.Session;
                if( session > 0 ) {
                    SessionContext context;
                    if( sessionDict.TryGetValue( session, out context ) ) {
                        sessionDict.Remove( session );
                        context.ResponseCallback( data );
                    }
                }
                else {
                    ServerRequest serverReq;
                    if( s2cRequest.TryGetValue( type, out serverReq ) ) {
                        serverReq( data );
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