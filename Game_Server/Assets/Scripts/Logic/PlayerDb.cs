using GameDatabase;
using GameProto;
using LitJson;

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

        public void Init() {



        }

        public int Create_Character( Player_Create_Character pkg ) {
            var existName = Database.Get<CharacterData>( x => x.Name == pkg.Name );
            if( existName == null ) {
                Data.Name = pkg.Name;

                //创建hero
                JsonData heros = new JsonData();
                for( int i = 0; i < 3; i++ ) {
                    JsonData hero = new JsonData {
                        ["Id"] = ( i + 1 ).ToString(),
                        ["Level"] = 1
                    };
                    heros.Add( hero );
                }
                Data.Heros = heros.ToJson();

                //默认物品
                JsonData items = new JsonData();
                for( int i = 0; i < 4; i++ ) {
                    JsonData item = new JsonData {
                        ["id"] = ( i + 1 ).ToString(),
                        ["count"] = 100
                    };
                    items.Add( item );
                }
                Data.Items = items.ToJson();

                Database.Update( data );//小写
                return 1;
            }
            return 3;//有重名
        }

        public int Upgrade_Level( Player_Upgrade_Level pkg ) {
            if( Data != null ) {


            }

            return 1;
        }

        public void Heartbeat() {


        }

    }
}
