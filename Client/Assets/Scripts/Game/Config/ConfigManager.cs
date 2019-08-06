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

    private Dictionary<string, JsonData> configDic = new Dictionary<string, JsonData>();

    JsonData LoadArrConf( string name ) {
        var confText = Resources.Load<TextAsset>( name );
        JsonData obj = JsonMapper.ToObject( confText.text );
        configDic.Add( name, obj );
        return obj;
    }

    public JsonData GetJsonDatas( string name ) {
        JsonData data;
        if( configDic.TryGetValue( name, out data ) == false ) {
            if( configData != null && configData[name] != null ) {
                data = configData[name];
                configDic.Add( name, data );
            }
            else {
                data = LoadArrConf( name );
                if( data != null ) {
                    configDic.Add( name, data );
                }
                else {
                    Debug.Log( "不存在配置 " + name );
                }
            }
        }
        return data;
    }

    //网上下载的配置
    private JsonData configData;

    public void SetConfigData( JsonData data ) {
        configData = data;
    }

}
