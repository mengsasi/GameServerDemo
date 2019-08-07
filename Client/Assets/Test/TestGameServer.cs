using Game.Network;
using GameProto;
using Google.Protobuf;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

public class TestGameServer : MonoBehaviour {

    //测试发包

    public Button BtnStartTcp;//开启tcp连接

    public Button BtnLoginTcp;//登陆tcp

    public InputField InputName;//角色名字
    public Button BtnCreateCharacter;//创建角色

    public Button BtnPlayerLevelUp;//玩家升级

    public Button BtnHeroLevelUp;//英雄升级

    public Button BtnUseItem;//使用物品

    //http登陆成功后，点击tcp登陆

    //登陆成功，看是否有角色，没有角色创建角色，有角色进入游戏

    //发其他包

    void Awake() {
        BtnStartTcp.onClick.AddListener( TcpConnect );
        BtnLoginTcp.onClick.AddListener( LoginTcp );
        BtnCreateCharacter.onClick.AddListener( CreateCharacter );
        BtnPlayerLevelUp.onClick.AddListener( PlayerLevelUp );
        BtnHeroLevelUp.onClick.AddListener( HeroLevelUp );
        BtnUseItem.onClick.AddListener( UseItem );
    }

    void Start() {

        //注册s2c的包
        //服务器主动推过来的消息
        //Sync_Character 当登陆成功后，服务器会将玩家数据发送过来
        NetworkManager.Instance.RegisterRequestCallback<Sync_Character>( ( data ) => {
            Sync_Character response = Utils.ParseByte<Sync_Character>( data );

            Debug.Log( "Character.Name " + response.Character.Name );

        } );

    }

    private void TcpConnect() {
        NetworkManager.TCPHostUrl = "127.0.0.1";
        NetworkManager.TCPPort = 50001;

        NetworkManager.Instance.StartConnection();
    }

    private void LoginTcp() {
        //测试假数据
        JsonData token = new JsonData {
            ["token"] = "anjainalls"
        };
        NetworkManager.LoginToken = token;//实际为http传回来的token

        NetworkManager.Instance.OnLoginFinish += ( login, success ) => {
            if( success ) {
                Debug.Log( "login success " + login.R );
                Debug.Log( "login Time " + login.Time );
            }
            else {
                Debug.Log( "login fail " + login.R );
            }
        };

        NetworkManager.Instance.TcpLogin();
    }

    private void CreateCharacter() {
        if( string.IsNullOrEmpty( InputName.text ) ) {
            Debug.Log( "填写角色昵称" );
            return;
        }
        Player_Create_Character request = new Player_Create_Character {
            Name = InputName.text
        };

        NetworkManager.Instance.DoRequest<Player_Create_Character>( request.ToByteArray(), ( data ) => {
            Player_Create_Character response = Utils.ParseByte<Player_Create_Character>( data );
            if( response.R == 1 ) {
                Debug.Log( "Player_Create_Character success " + response.R );
            }
            else {
                Debug.Log( "Player_Create_Character fail " + response.R );
            }
        } );
    }

    private void PlayerLevelUp() {
        Player_Upgrade_Level request = new Player_Upgrade_Level();

        Debug.Log( "Player_Upgrade_Level" );

        NetworkManager.Instance.DoRequest<Player_Upgrade_Level>( request.ToByteArray(), ( data ) => {
            Player_Upgrade_Level response = Utils.ParseByte<Player_Upgrade_Level>( data );
            if( response.R == 1 ) {
                Debug.Log( "Player_Upgrade_Level success " + response.R );
            }
            else {
                Debug.Log( "Player_Upgrade_Level fail " + response.R );
            }
        } );
    }

    private void HeroLevelUp() {



    }

    private void UseItem() {



    }

}
