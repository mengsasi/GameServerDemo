using Core;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Server {

    public class MainServer : Singleton<MainServer> {

        /// <summary>
        /// 监听套接字
        /// </summary>
        public Socket Watchdog;

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int maxSessionClient = 50;

        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="por"></param>
        public void StartServer( string host, int port ) {
            NetworkPool.SetMaxSessionClient( maxSessionClient );
            Watchdog = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            IPAddress ipAddress = IPAddress.Parse( host );
            IPEndPoint ipEndPoint = new IPEndPoint( ipAddress, port );
            Watchdog.Bind( ipEndPoint );
            Watchdog.Listen( maxSessionClient );
            Watchdog.BeginAccept( AcceptCallBack, null );

            Debug.LogFormat( "服务器启动成功! host:{0} port:{1}", host, port );
        }

        /// <summary>
        /// 异步建立客户端连接回调
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptCallBack( IAsyncResult ar ) {
            try {
                Socket socket = Watchdog.EndAccept( ar );
                NetworkTcpClient session = NetworkPool.GetSessionClient();

                if( session == null ) {
                    socket.Close();
                    Debug.Log( "连接已满！" );
                }
                else {
                    UnityMainThreadDispatcher.Instance.Enqueue( () => {
                        session.Init( socket );
                        string address = session.GetRemoteAddress();
                        Debug.Log( "客户端连接 " + address );
                    } );
                }

                Watchdog.BeginAccept( AcceptCallBack, null );
            }
            catch( Exception e ) {
                Debug.Log( "异步建立客户端连接失败：" + e.Message );
            }
        }
    }
}
