using LitJson;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager {

    private static ConfigManager instance;
    public static ConfigManager Instance {
        get {
            return instance ?? ( instance = new ConfigManager() );
        }
    }

    private Dictionary<string, IList<JsonData>> configDic = new Dictionary<string, IList<JsonData>>();

    IList<JsonData> LoadArrConf( string name ) {
        var confText = Resources.Load<TextAsset>( name );
        IList<JsonData> obj = JsonMapper.ToObject( confText.text ).ValueAsArray();
        configDic.Add( name, obj );
        return obj;
    }

    //本地配置测试用，实际应该是从服务器下载下来的配置
    //ConfigManager() {
    //    var confText = Resources.Load<TextAsset>( "allConfig" );
    //    SetConfigData( JsonMapper.ToObject( confText.text ) );
    //}

    public IList<JsonData> GetJsonDatas( string name ) {
        IList<JsonData> data;
        if( configDic.TryGetValue( name, out data ) == false ) {
            if( configData != null && configData[name] != null ) {
                data = configData[name].ValueAsArray();
                configDic.Add( name, data );
            }
            else {
                Debug.Log( "不存在配置 " + name );
            }
        }
        return data;
    }

    private JsonData configData;

    public void SetConfigData( JsonData data ) {
        configData = data;
    }

}
