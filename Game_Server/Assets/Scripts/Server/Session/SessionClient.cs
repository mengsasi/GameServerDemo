using System;
using System.Net.Sockets;
using UnityEngine;

namespace Server {

    public class SessionClient {

        /// <summary>
        /// 缓冲区大小
        /// </summary>
        private const int BUFFER_SIZE = 1024;

        /// <summary>
        /// 读数据缓冲区
        /// </summary>
        private byte[] readBuffer = new byte[BUFFER_SIZE];

        /// <summary>
        /// 套接字
        /// </summary>
        private Socket socket;

        /// <summary>
        /// 是否使用
        /// </summary>
        public bool isUse = false;

        /// <summary>
        /// 动态缓冲区
        /// </summary>


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
            //string address = GetRemoteAddress();
            socket.Close();

            //buffer.clear();
            isUse = false;
        }

        public void Send() {
            try {

                //socket.Send( null );
            }
            catch( Exception e ) {
                Debug.Log( e.Message );
            }
        }

        private void ReceiveCallBack( IAsyncResult ar ) {
            try {
                int count = socket.EndReceive( ar );
                if( count <= 0 ) {
                    Debug.Log( GetRemoteAddress() + "断开连接" );
                    Close();
                    return;
                }

                //数据解析

                socket.BeginReceive( readBuffer, 0, readBuffer.Length, SocketFlags.None, ReceiveCallBack, null );
            }
            catch( Exception ) {
                Debug.Log( GetRemoteAddress() + "断开连接" );
                Close();
            }

        }

    }
}
