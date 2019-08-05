using System.Collections.Generic;
using UnityEngine;

namespace Server {

    public class NetworkManager : Singleton<NetworkManager> {

        private Dictionary<long, NetworkProtoClient> clientDict = new Dictionary<long, NetworkProtoClient>();

        public NetworkProtoClient InitProtoClient( long id, NetworkTcpClient client ) {
            var obj = new GameObject( "client " + id );
            var protoClient = obj.AddComponent<NetworkProtoClient>();
            obj.transform.SetParent( NetworkManager.Instance.transform );
            clientDict.Add( id, protoClient );
            protoClient.Init( client );
            return protoClient;
        }

        public void RemoveClient( long id ) {
            if( clientDict.ContainsKey( id ) ) {
                clientDict.Remove( id );
            }
        }





    }

}