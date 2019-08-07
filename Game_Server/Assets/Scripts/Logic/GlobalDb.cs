using GameProto;
using Google.Protobuf;

namespace Logic {

    public class GlobalDb {

        private PlayerDb playerDb;

        public static GlobalDb New( PlayerDb player, params string[] id ) {
            var global = new GlobalDb {
                playerDb = player
            };
            return global;
        }

        public byte[] Use_Item( string id, int count ) {
            Global_Use_Item ret = new Global_Use_Item();
            if( id == "1" || id == "2" || id == "3" ) {
                if( playerDb.Single_Use_Item( id, count ) ) {
                    ret.R = 1;
                    playerDb.Dirty( ret.Items );
                }
                else {
                    ret.R = 2;
                }
            }
            else {
                ret.R = 3;
            }
            return ret.ToByteArray();
        }

    }

}

