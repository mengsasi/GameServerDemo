using GameProto;
using Google.Protobuf;
using UnityEngine;

namespace Server {

    public class NetworkProtoClient {

        public long ID;

        private NetworkTcpClient client;

        public long Session {
            get;
            set;
        }

        public void Init( NetworkTcpClient client ) {
            Session = 0;
            this.client = client;
            ID = client.ID;
            PlayerManager.Instance.On_Connect( client.ID, this );
        }

        public long DoRequest<T>( byte[] data, long session = -1 ) {
            //服务器主动发的，是-1
            //客户端发过来，有session，返回的根据客户端的session值
            long sess = session;
            var type = Utils.GetProtpType<T>();
            if( session == -1 ) {
                Session = Session + 1;
                if( Session > 65535 ) {
                    Session = 0;
                }
                sess = Session;
                type = NetworkUtils.Instance.ProtoTags[typeof( T )];
            }

            Package_Head pkg = new Package_Head {
                Type = type,
                Session = sess
            };

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

        public void Update() {
            if( client == null )
                return;
            var datas = client.Dispatch();
            if( datas != null ) {
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

                PlayerManager.Instance.On_Request( client.ID, pkg, data );
            }
        }

        public void Close() {


        }

    }
}
