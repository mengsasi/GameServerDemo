using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Server {

    public class NetworkTcpClient {

        public long ID;

        /// <summary>
        /// 套接字
        /// </summary>
        private Socket socket;

        private Queue<byte[]> recvQueue = new Queue<byte[]>();

        /// <summary>
        /// 缓冲区大小
        /// </summary>
        private const int BUFFER_SIZE = 1024;

        /// <summary>
        /// 读数据缓冲区
        /// </summary>
        private byte[] readBuffer = new byte[BUFFER_SIZE];

        /// <summary>
        /// 是否使用
        /// </summary>
        public bool isUse = false;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="receiveTimeout"></param>
        public void Init( Socket socket, int receiveTimeout = 1000 ) {
            this.socket = socket;
            this.socket.ReceiveTimeout = receiveTimeout;
            isUse = true;

            socket.BeginReceive( readBuffer, 0, readBuffer.Length, SocketFlags.None, ReceiveCallBack, null );

            NetworkPool.ClientCount++;
            if( NetworkPool.ClientCount > 65535 ) {
                NetworkPool.ClientCount = 1;
            }
            ID = NetworkPool.ClientCount;

            NetworkManager.Instance.InitProtoClient( ID, this );
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
            readBuffer = new byte[BUFFER_SIZE];
            recvQueue.Clear();
            isUse = false;
        }

        public byte[] Dispatch() {
            lock( recvQueue ) {
                if( recvQueue.Count == 0 )
                    return null;

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

        private void ReceiveCallBack( IAsyncResult result ) {
            try {
                int length = socket.EndReceive( result );
                if( length <= 0 ) {
                    Debug.Log( GetRemoteAddress() + "断开连接" );
                    Close();
                    return;
                }

                var buffer = readBuffer;
                int typeLength = buffer[0];
                int dataLength = buffer[1];

                if( dataLength > 0 ) {
                    int blength = buffer.Length;
                    byte[] data = new byte[blength];

                    for( int i = 0; i < blength; i++ ) {
                        data[i] = buffer[i];
                    }

                    lock( recvQueue ) {
                        recvQueue.Enqueue( data );
                    }
                }

                readBuffer = new byte[BUFFER_SIZE];
                socket.BeginReceive( readBuffer, 0, readBuffer.Length, SocketFlags.None, ReceiveCallBack, null );
            }
            catch( Exception ) {
                Debug.Log( GetRemoteAddress() + "断开连接" );
                Close();
            }

        }

    }

}
