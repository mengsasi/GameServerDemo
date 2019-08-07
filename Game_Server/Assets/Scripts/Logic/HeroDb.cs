using Config;
using GameDatabase;
using GameProto;
using Google.Protobuf;
using UnityEngine;

namespace Logic {

    public class HeroDb {

        private PlayerDb playerDb;

        private HeroData data;
        public HeroData Data {
            get {
                return data;
            }
        }

        public static HeroDb New( PlayerDb player, HeroData data ) {
            var hero = new HeroDb {
                playerDb = player
            };
            hero.data = data;
            return hero;
        }

        public byte[] Upgrade_Level( Hero_Upgrade_Level pkg ) {
            Hero_Upgrade_Level ret = new Hero_Upgrade_Level();
            if( Data != null ) {

                Debug.Log( "Upgrade_Level " );

                var next = Data.Level + 1;
                var conf = LevelConfigs.Instance.GetConf( next );
                if( conf == null ) {
                    ret.R = 3;
                }
                else {
                    if( playerDb.Single_Use_Item( conf.Cost.Id, conf.Cost.Count ) ) {
                        Data.Level++;
                        playerDb.Save();
                        ret.R = 1;
                        playerDb.Dirty( ret.Items );
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

    }

}
