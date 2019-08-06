using GameProto;
using System;
using System.Collections.Generic;

namespace Core.Network {

    public class NetworkUtils {

        private static NetworkUtils instance;
        public static NetworkUtils Instance {
            get {
                return instance ?? ( instance = new NetworkUtils() );
            }
        }

        public Dictionary<Type, string> ProtoTags = new Dictionary<Type, string>();

        NetworkUtils() {
            ProtoTags.Add( typeof( Sync_Character ), "Sync_Character" );

        }

    }
}
