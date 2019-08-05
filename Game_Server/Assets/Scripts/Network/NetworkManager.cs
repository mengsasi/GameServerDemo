using GameProto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Server {

    public class NetworkManager : Singleton<NetworkManager> {

        NetworkManager() {
            InitTag();
        }

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











        public static Dictionary<Type, int> ProtoTags = new Dictionary<Type, int>();

        public static void InitTag() {
            ProtoTags.Add( typeof( Sync_Character ), 1000 );
        }





    }

}