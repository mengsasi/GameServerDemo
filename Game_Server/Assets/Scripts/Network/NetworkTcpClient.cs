using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

namespace Server {

    public class NetworkTcpClient {

        public long ID;
        private Socket socket;
        public Queue<byte[]> recvQueue = new Queue<byte[]>();
        private ProtoStream recvStream = new ProtoStream();

        /// <summary>
        /// 是否使用
        /// </summary>
        public bool isUse = false;

        public NetworkTcpClient() {
            InitStream();
        }

        private void InitStream() {
            byte[] receiveBuffer = new byte[1 << 16];
            recvStream.Write( receiveBuffer, 0, receiveBuffer.Length );
            recvStream.Seek( 0, SeekOrigin.Begin );
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="receiveTimeout"></param>
        public void Init( Socket socket, int receiveTimeout = 1000 ) {
            this.socket = socket;
            this.socket.ReceiveTimeout = receiveTimeout;
            isUse = true;

            Begin_Recv();

            NetworkPool.ClientCount++;
            if( NetworkPool.ClientCount > 65535 ) {
                NetworkPool.ClientCount = 1;
            }
            ID = NetworkPool.ClientCount;

            var protoClient = new NetworkProtoClient();
            protoClient.Init( this );
            Core.TickerManager.RegisterTicker( protoClient );
        }

        /// <summary>
        /// 获取远程套接字地址
        /// </summary>
        /// <returns></returns>
        public string GetRemoteAddress() {
            if( !isUse ) {
                return null;
            }
            return socket.RemoteEndPoint.ToString();
        }

        /// <summary>
        /// 获取本地套接字地址
        /// </summary>
        /// <returns></returns>
        public string GetLocalAddress() {
            if( !isUse ) {
                return null;
            }
            return socket.LocalEndPoint.ToString();
        }

        public void Close() {
            if( !isUse ) {
                return;
            }
            if( socket != null && socket.Connected ) {
                try {
                    socket.Shutdown( SocketShutdown.Both );
                    socket.Close();
                    socket = null;
                }
                catch {
                    socket = null;
                }
            }
            recvStream = new ProtoStream();
            InitStream();
            recvQueue.Clear();
            isUse = false;
            Logic.PlayerManager.Instance.Clear( ID );
        }

        public byte[] Dispatch() {
            lock( recvQueue ) {
                if( recvQueue.Count == 0 ) {
                    return null;
                }
                return recvQueue.Dequeue();
            }
        }

        public void Destroy() {
            Close();
        }

        public void Send( byte[] data, int length ) {
            try {
                int byteSent = socket.Send( data, 0, length, SocketFlags.None );
                Debug.Assert( byteSent == length );
            }
            catch( Exception e ) {
                Debug.Log( e.Message );
            }
        }

        private int receivePosition;

        private void Begin_Recv() {
            if( socket != null && socket.Connected ) {
                socket.BeginReceive( recvStream.Buffer, receivePosition,
                    recvStream.Buffer.Length - receivePosition, SocketFlags.None, ( result ) => {
                        try {
                            int length = socket.EndReceive( result );
                            if( length <= 0 ) {
                                Debug.Log( GetRemoteAddress() + "断开连接" );
                                Close();
                                return;
                            }

                            receivePosition += length;

                            int i = recvStream.Position;
                            while( receivePosition >= i + 2 ) {
                                int dataLength = recvStream[i] + recvStream[i + 1];

                                int sz = dataLength + 2;
                                if( receivePosition < i + sz ) {
                                    break;
                                }

                                recvStream.Seek( 0, SeekOrigin.Current );

                                if( dataLength > 0 ) {
                                    byte[] data = new byte[sz];
                                    recvStream.Read( data, 0, sz );
                                    lock( recvQueue ) {
                                        recvQueue.Enqueue( data );
                                    }
                                }

                                i += sz;
                            }

                            if( receivePosition == recvStream.Buffer.Length ) {
                                recvStream.Seek( 0, SeekOrigin.End );
                                recvStream.MoveUp( i, i );
                                receivePosition = recvStream.Position;
                                recvStream.Seek( 0, SeekOrigin.Begin );
                            }

                            Begin_Recv();
                        }
                        catch( Exception ) {
                            Debug.Log( GetRemoteAddress() + "断开连接" );
                            Close();
                        }
                    }, null );
            }
        }

    }

}
