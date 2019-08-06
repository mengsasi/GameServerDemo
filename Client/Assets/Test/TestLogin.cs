using UnityEngine;
using UnityEngine.UI;

public class TestLogin : MonoBehaviour {

    public Button BtnCheck;

    public Button BtnLogin;

    public Button BtnRefresh;

    //完整步骤
    //首先获取版本号
    //get config-version
    //根据版本号，下载配置，游戏更新等操作

    //然后进行账号登陆
    //判断本地是否有之前登陆过的账号数据，（之前账号登陆成功之后，需要保存一个token在本地）
    //如果有，就用这个token，post刷新token，refreshtoken
    //如果没有账号，是新设备，或者新创建的账号
    //就post，login 传的code参数是玩家用户名
    //  //登陆成功，需要将LoginToken变量保存到本地，下次登陆只需要验证token，而不用重复输入用户名
    //  //除非想做切换账号的功能，切换账号，实际就是清除本地记录，重新发login

    void Awake() {
        BtnCheck.onClick.AddListener( CheckVersion );
        BtnLogin.onClick.AddListener( DoHttpLogin );
        BtnRefresh.onClick.AddListener( Refresh );

    }

    public void CheckVersion() {
        GameLogin.CheckVersion( ( status, data ) => {
            Debug.Log( status == HttpManager.HttpResponseStatus.OK ? "success" : "fail" );
        } );
    }

    public void DoHttpLogin() {
        GameLogin.HttpLogin( "123", ( status, data ) => {
            if( status == HttpManager.HttpResponseStatus.OK ) {
                Debug.Log( data["token"].ValueAsString() );
            }
            else {
                Debug.Log( "fail" );
            }
        } );
    }

    public void Refresh() {
        string oldToken = global::GameLogin.JsonToken["token"].ValueAsString();
        GameLogin.RefreshToken( oldToken, ( status, data ) => {
            if( status == HttpManager.HttpResponseStatus.OK ) {
                Debug.Log( data["token"].ValueAsString() );
            }
            else {
                Debug.Log( "fail" );
            }
        } );
    }

}
