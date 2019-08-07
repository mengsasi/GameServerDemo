using GameProto;
using UnityEngine;

namespace Logic {

    public class GlobalProcessor {

        public static byte[] Use_Item( PlayerDb player, byte[] data ) {
            Global_Use_Item request = Utils.ParseByte<Global_Use_Item>( data );
            Debug.Assert( !string.IsNullOrEmpty( request.Id ) && request.Count > 0 );
            return player.globalDb.Use_Item( request.Id, request.Count );
        }

    }

}
