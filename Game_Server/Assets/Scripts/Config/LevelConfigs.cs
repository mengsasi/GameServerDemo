using GameDatabase;
using System.Collections;
using System.Collections.Generic;

namespace Config {

    //升级配置
    public class LevelConfig {
        public int Level;
        public ItemData Cost;
    }

    public class LevelConfigs {

        private static LevelConfigs instance;
        public static LevelConfigs Instance {
            get {
                return instance ?? ( instance = new LevelConfigs() );
            }
        }

        private Dictionary<int, LevelConfig> confDict = new Dictionary<int, LevelConfig>();

        LevelConfigs() {
            var data = ConfigManager.Instance.GetJsonDatas( "Level.json" );
            IDictionary confs = data as IDictionary;
            foreach( var item in confs.Keys ) {
                var token = data[item.ToString()];
                LevelConfig conf = new LevelConfig();
                var level = token["_id"].ValueAsString();
                conf.Level = int.Parse( level );

                var cost = token["cost"];
                var costid = cost["id"].ValueAsString();
                var costcount = cost["count"].ValueAsInt();
                var itemdata = new ItemData() {
                    Id = costid,
                    Count = costcount
                };
                conf.Cost = itemdata;
                confDict.Add( conf.Level, conf );
            }
        }

        public LevelConfig GetConf( int level ) {
            LevelConfig conf;
            confDict.TryGetValue( level, out conf );
            return conf;
        }

    }

}