using GameDatabase;
using GameProto;

namespace Logic {

    public class PlayerDb {

        private string playerId;

        private CharacterData data;
        private CharacterData Data {
            get {
                if( data == null ) {
                    data = Database.Get<CharacterData>( x => x.Id == playerId );
                }
                return data;
            }
            set {
                data = value;
            }
        }

        public static PlayerDb New( string id ) {
            var player = new PlayerDb {
                playerId = id
            };
            var exist = Database.Get<CharacterData>( x => x.Id == id );
            if( exist == null ) {
                CharacterData data = new CharacterData() {
                    Id = id
                };
                Database.Insert( data );
            }
            return player;
        }

        public int Create_Character( Player_Create_Character pkg ) {
            var existName = Database.Get<CharacterData>( x => x.Name == pkg.Name );
            if( existName == null ) {
                Data.Name = pkg.Name;
                Database.Update( data );
                return 1;
            }
            return 3;//有重名
        }

        public int Upgrade_Level( Player_Upgrade_Level pkg ) {


            return 1;
        }


    }
}
