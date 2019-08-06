using GameProto;
using Google.Protobuf;
using Logic;
using System.IO;
using UnityEngine;

namespace Server {

    public class NetworkProtoClient {

        public long ID;

        private NetworkTcpClient client;

        private ProtoStream sendStream = new ProtoStream();

        private static readonly int MAX_PACK_LEN = ( 1 << 16 ) - 1;

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

        public void Update() {
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

                PlayerManager.Instance.On_Request( client.ID, pkg, data );

                datas = client.Dispatch();
            }
        }

        public void Close() {


        }

    }
}
