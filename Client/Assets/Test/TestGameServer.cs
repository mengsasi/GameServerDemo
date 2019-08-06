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

    private void TcpConnect() {
        NetworkManager.TCPHostUrl = "127.0.0.1";
        NetworkManager.TCPPort = 50001;

        NetworkManager.Instance.StartConnection();
    }

    private void LoginTcp() {
        //测试
        JsonData token = new JsonData();
        token["token"] = "anjainlls";
        NetworkManager.LoginToken = token;

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


    }

    private void HeroLevelUp() {



    }

    private void UseItem() {






    }




}
