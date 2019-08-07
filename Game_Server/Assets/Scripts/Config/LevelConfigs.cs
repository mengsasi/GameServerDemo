using GameDatabase;
using LitJson;
using System.Collections.Generic;

namespace Config {

    //升级配置
    //为了简单，玩家升级和hero升级就都走一个升级配置了
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
            var data = ConfigManager.Instance.GetJsonDatas( "Level" );
            foreach( var item in data ) {
                LevelConfig conf = new LevelConfig();
                var level = item["_id"].ValueAsString();
                conf.Level = int.Parse( level );

                string cost = item["cost"].ValueAsString();
                JsonData costJson = JsonMapper.ToObject( cost );
                var costid = costJson["id"].ValueAsString();
                var costcount = costJson["count"].ValueAsInt();
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