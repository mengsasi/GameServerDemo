using GameProto;
using UnityEngine;

namespace Logic {

    public class HeroProcessor {

        public static byte[] Upgrade_Level( PlayerDb player, byte[] data ) {
            Hero_Upgrade_Level request = Utils.ParseByte<Hero_Upgrade_Level>( data );
            Debug.Assert( !string.IsNullOrEmpty( request.Id ) );
            var herodb = player.heroDb[request.Id];
            return herodb.Upgrade_Level( request );
        }

    }

}
