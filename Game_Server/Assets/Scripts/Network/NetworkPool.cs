using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Server {

    public class NetworkPool : Singleton<NetworkPool> {

        public static long ClientCount = 0;

        /// <summary>
        /// 会话池
        /// </summary>
        private static ConcurrentBag<NetworkTcpClient> Clients = new ConcurrentBag<NetworkTcpClient>();

        /// <summary>
        /// 设置池最大数量
        /// </summary>
        /// <param name="count"></param>
        public static void SetMaxSessionClient( int count ) {
            Clients = new ConcurrentBag<NetworkTcpClient>();
            for( int i = 0; i < count; i++ ) {
                Clients.Add( new NetworkTcpClient() );
            }
        }

        /// <summary>
        /// 获取空会话
        /// </summary>
        /// <returns></returns>
        public static NetworkTcpClient GetSessionClient() {
            foreach( var session in Clients ) {
                if( !session.isUse ) {
                    return session;
                }
            }
            return null;
        }

        public static IEnumerable<NetworkTcpClient> GetOnlineSession() {
            List<NetworkTcpClient> list = new List<NetworkTcpClient>();
            foreach( var session in GetEnumerable() ) {
                if( session.isUse ) {
                    list.Add( session );
                }
            }
            return list;
        }

        public static IEnumerable<NetworkTcpClient> GetEnumerable() {
            return Clients;
        }

    }
}
