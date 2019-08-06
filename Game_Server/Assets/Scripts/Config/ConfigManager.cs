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


}
