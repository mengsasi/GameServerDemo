using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Server {

    public class SessionPool {

        /// <summary>
        /// 会话池
        /// </summary>
        private static ConcurrentBag<SessionClient> Sessions = new ConcurrentBag<SessionClient>();

        /// <summary>
        /// 设置池最大数量
        /// </summary>
        /// <param name="count"></param>
        public static void SetMaxSessionClient( int count ) {
            Sessions = new ConcurrentBag<SessionClient>();
            for( int i = 0; i < count; i++ ) {

                Sessions.Add( new SessionClient() );
            }
        }

        /// <summary>
        /// 获取空会话
        /// </summary>
        /// <returns></returns>
        public static SessionClient GetSessionClient() {
            foreach( var session in Sessions ) {
                if( !session.isUse ) {
                    return session;
                }
            }
            return null;
        }

        public static IEnumerable<SessionClient> GetOnlineSession() {
            List<SessionClient> list = new List<SessionClient>();
            foreach( var session in GetEnumerable() ) {
                if( session.isUse ) {
                    list.Add( session );
                }
            }
            return list;
        }

        public static IEnumerable<SessionClient> GetEnumerable() {
            return Sessions;
        }

    }
}
