using Core.Network;
using LitJson;
using System;

public class GameLogin {

    private static string Server_URL = "http://127.0.0.1:13200";

    private static readonly string Login_Path = "/login";
    private static readonly string RefreshToken_Path = "/refreshtoken";
    private static readonly string Config_Version_Path = "/config-version";
    private static readonly string All_Config = "/all-config";

    public static int Version = -1;

    //先检查配置版本
    public static void CheckVersion( Action<HttpManager.HttpResponseStatus, JsonData> callback ) {
        HttpManager.Instance.GetJson( Server_URL + Config_Version_Path, ( status, data ) => {
            if( status == HttpManager.HttpResponseStatus.OK ) {
                try {
                    Version = data["version"].ValueAsInt();
                }
                catch {

                }
            }
            if( callback != null ) {
                callback( status, data );
            }
        } );
    }

    //下载配置
    public static void GetConfigs( Action<HttpManager.HttpResponseStatus, JsonData> callback ) {
        HttpManager.Instance.GetJson( Server_URL + All_Config + "?v=" + Version, ( status, data ) => {
            if( status == HttpManager.HttpResponseStatus.OK ) {
                try {
                    ConfigManager.Instance.SetConfigData( data );
                }
                catch {

                }
            }
            if( callback != null ) {
                callback( status, data );
            }
        } );
    }

    public static JsonData JsonToken;

    //code 玩家用户名
    public static void HttpLogin( string code, Action<HttpManager.HttpResponseStatus, JsonData> callback ) {
        var body = new {
            code = code,
            sdk = "debug",
            version = Version
        };

        HttpManager.Instance.PostJson( Server_URL + Login_Path, JsonMapper.ToJson( body ), ( status, data ) => {
            if( status == HttpManager.HttpResponseStatus.OK ) {
                //登陆验证，token
                JsonToken = data;
                if( callback != null ) {
                    callback( status, data );
                }
            }

        } );
    }

    public static void RefreshToken( string rtoken, Action<HttpManager.HttpResponseStatus, JsonData> callback ) {
        var body = new {
            token = rtoken
        };
        HttpManager.Instance.PostJson( Server_URL + RefreshToken_Path, JsonMapper.ToJson( body ), ( status, data ) => {
            if( status == HttpManager.HttpResponseStatus.OK ) {
                var r = data["r"].ValueAsInt();
                if( r == 0 ) {
                    //返回r = 0，刷新成功，登陆成功
                    JsonToken = data;
                    if( callback != null ) {
                        callback( status, data );
                    }
                }
                else if( r == 1 ) {
                    //返回r = 1，账号不存在，重新用Login输入用户名登陆
                    if( callback != null ) {
                        callback( status, null );
                    }
                }
            }
        } );
    }

}
