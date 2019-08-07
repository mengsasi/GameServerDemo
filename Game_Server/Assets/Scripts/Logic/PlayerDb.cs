using Config;
using Core;
using GameDatabase;
using GameProto;
using Google.Protobuf;
using Google.Protobuf.Collections;
using LitJson;
using System.Collections.Generic;
using UnityEngine;

namespace Logic {

    public class PlayerDb : ITicker {

        public PlayerDb playerDb;
        public Dictionary<string, HeroDb> heroDb = new Dictionary<string, HeroDb>();
        public GlobalDb globalDb;

        private string id;
        public string Id {
            get {
                return id;
            }
            set {
                id = value;
            }
        }

        private CharacterData data;
        public CharacterData Data {
            get {
                if( data == null ) {
                    data = Database.Get<CharacterData>( x => x.Id == Id );
                }
                return data;
            }
            set {
                data = value;
            }
        }

        public static PlayerDb New( string id ) {
            var player = new PlayerDb {
                Id = id
            };
            var exist = Database.Get<CharacterData>( x => x.Id == id );
            if( exist == null ) {
                CharacterData data = new CharacterData() {
                    Id = id
                };
                Database.Insert( data );
            }
            TickerManager.RegisterSecondTicker( player );
            return player;
        }

        public int Create_Character( Player_Create_Character pkg ) {
            var existName = Database.Get<CharacterData>( x => x.Name == pkg.Name );
            if( existName == null ) {
                Data.Name = pkg.Name;
                Data.Level = 1;

                //创建hero
                JsonData heros = new JsonData();
                for( int i = 0; i < 3; i++ ) {
                    JsonData hero = new JsonData {
                        ["id"] = ( i + 1 ).ToString(),
                        ["level"] = 1
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

        public void Init() {
            this.playerDb = this;
            this.globalDb = GlobalDb.New( this );

            Data.GetItems();

            this.heroDb.Clear();
            var heros = Data.GetHeros();
            for( int i = 0; i < heros.Count; i++ ) {
                this.heroDb.Add( heros[i].Id, HeroDb.New( this, heros[i] ) );
            }
            Data.ListHero = heros;

            Save();
            Sync_Character pkg = new Sync_Character {
                Character = GetPkgData()
            };
            PlayerManager.Instance.Send_Request<Sync_Character>( Id, pkg.ToByteArray() );
        }

        public void Save() {
            Data.Items = Data.GetItemJson();
            Data.Heros = Data.GetHeroJson();
            Database.Update( data );//小写
        }

        public Character GetPkgData() {
            Character cha = new Character {
                Id = Data.Id,
                Name = Data.Name,
                Level = Data.Level
            };
            for( int i = 0; i < Data.ListHero.Count; i++ ) {
                var item = Data.ListHero[i];
                cha.Heros.Add( new Hero() {
                    Id = item.Id,
                    Level = item.Level
                } );
            }
            for( int i = 0; i < Data.ListItem.Count; i++ ) {
                var item = Data.ListItem[i];
                cha.Items.Add( new Item() {
                    Id = item.Id,
                    Count = item.Count
                } );
            }
            return cha;
        }

        public List<Item> itemDirty = new List<Item>();

        public int ItemCount( string id ) {
            var items = Data.GetItems();
            var exist = items.Find( ( item ) => item.Id == id );
            if( exist != null ) {
                return exist.Count;
            }
            return 0;
        }

        public bool Single_Use_Item( string id, int count ) {
            var items = Data.GetItems();
            var exist = items.Find( ( item ) => item.Id == id );
            if( exist != null ) {
                if( exist.Count >= count ) {
                    exist.Count -= count;
                    itemDirty.Add( new Item() {
                        Id = id,
                        Count = exist.Count
                    } );
                    return true;
                }
            }
            return false;
        }

        public void Dirty( RepeatedField<Item> list ) {
            if( itemDirty.Count > 0 ) {
                for( int i = 0; i < itemDirty.Count; i++ ) {
                    list.Add( itemDirty[i] );
                }
                itemDirty.Clear();
            }
        }

        public byte[] Upgrade_Level( Player_Upgrade_Level pkg ) {
            Player_Upgrade_Level ret = new Player_Upgrade_Level();
            if( Data != null ) {
                var next = Data.Level + 1;
                var conf = LevelConfigs.Instance.GetConf( next );
                if( conf == null ) {
                    ret.R = 3;
                }
                else {
                    if( Single_Use_Item( conf.Cost.Id, conf.Cost.Count ) ) {
                        Data.Level++;
                        ret.R = 1;
                        Dirty( ret.Items );
                    }
                    else {
                        ret.R = 2;
                    }
                }
            }
            else {
                ret.R = 4;
            }
            return ret.ToByteArray();
        }

        public void Exit() {
            TickerManager.UnRegisterSecondTicker( this );
            Save();
            PlayerManager.Instance.UnRegister( Id );
        }

        public int tick = 0;

        public void Heartbeat() {
            tick = 0;
        }

        public void Update() {
            tick++;
            if( tick >= 20 ) {
                Debug.Log( "tick > 20 " + Id );
                tick = 0;
            }
        }
    }
}
