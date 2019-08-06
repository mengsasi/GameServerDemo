using System.Collections.Generic;
using UnityEngine;

namespace Server {

    public class NetworkManager : Singleton<NetworkManager> {

        private List<NetworkProtoClient> clients = new List<NetworkProtoClient>();

        private bool isInit = false;

        public void Init() {
            isInit = true;
        }

        public void DeInit() {
            isInit = false;
            clients.Clear();
        }

        public void InitProtoClient( long id, NetworkTcpClient client ) {
            var protoClient = new NetworkProtoClient();
            protoClient.Init( client );
            Register( protoClient );
        }

        public void Register( NetworkProtoClient client ) {
            var exits = clients.Find( ( item ) => item.ID == client.ID );
            if( exits == null ) {
                clients.Add( client );
            }
            else {
                Debug.Log( "client 重复" );
            }
        }

        public void UnRegister( long id ) {
            var exits = clients.Find( ( item ) => item.ID == id );
            if( exits != null ) {
                clients.Remove( exits );
            }
        }

        void Update() {
            if( isInit && clients.Count > 0 ) {
                for( int i = clients.Count - 1; i >= 0; i-- ) {
                    var item = clients[i];
                    if( item != null ) {
                        item.Update();
                    }
                }
            }
        }

    }
}