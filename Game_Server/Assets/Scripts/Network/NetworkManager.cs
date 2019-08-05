using System.Collections.Generic;
using UnityEngine;

namespace Server {

    public class NetworkManager {

        private static NetworkManager instance;
        public static NetworkManager Instance {
            get {
                return instance ?? ( instance = new NetworkManager() );
            }
        }

        private GameObject netNode;
        public GameObject NetNode {
            get {
                if( netNode == null ) {
                    netNode = new GameObject( "NetNode" );
                    Object.DontDestroyOnLoad( netNode );
                }
                return netNode;
            }
        }

        private Dictionary<long, NetworkProtoClient> clientDict = new Dictionary<long, NetworkProtoClient>();

        public void InitProtoClient( long id, NetworkTcpClient client ) {
            var obj = new GameObject( "client " + id );
            var protoClient = obj.AddComponent<NetworkProtoClient>();
            obj.transform.SetParent( NetNode.transform );
            clientDict.Add( id, protoClient );
            protoClient.Init( client );
        }

        public void RemoveClient( long id ) {
            if( clientDict.ContainsKey( id ) ) {
                clientDict.Remove( id );
            }
        }

    }

}