using System.Collections.Generic;

//升级配置
public class LevelConfig {
    public int Level;
    public ItemData Cost;
}

public class ConfigManager {

    private static ConfigManager instance;
    public static ConfigManager Instance {
        get {
            return instance ?? ( instance = new ConfigManager() );
        }
    }

    //初始1级，最高等级5级
    public List<LevelConfig> LevelConfigs = new List<LevelConfig>() {
        //升到2级的消耗
         new LevelConfig() {
            Level = 2,
            Cost = new ItemData() {
                Id = "item2",
                Count = 1
            }
        },
        new LevelConfig() {
            Level = 3,
            Cost = new ItemData() {
                Id = "item3",
                Count = 1
            }
        },
        new LevelConfig() {
            Level = 4,
            Cost = new ItemData() {
                Id = "item4",
                Count = 1
            }
        },
        new LevelConfig() {
            Level = 5,
            Cost = new ItemData() {
                Id = "item5",
                Count = 1
            }
        }
    };

    public LevelConfig GetLevelConfig( int level ) {
        return LevelConfigs.Find( ( item ) => item.Level == level );
    }

}
