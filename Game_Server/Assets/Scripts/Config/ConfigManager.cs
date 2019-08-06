using LitJson;
using System.Collections.Generic;
using UnityEngine;

namespace Config {

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
                data = LoadArrConf( name );
                configDic.Add( name, data );
            }
            return data;
        }

    }

}
