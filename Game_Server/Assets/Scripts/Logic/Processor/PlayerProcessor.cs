using GameProto;

namespace Logic {

    public class PlayerProcessor {

        public static byte[] Upgrade_Level( PlayerDb player, byte[] data ) {
            Player_Upgrade_Level request = Utils.ParseByte<Player_Upgrade_Level>( data );
            return player.Upgrade_Level( request );
        }

    }

}
