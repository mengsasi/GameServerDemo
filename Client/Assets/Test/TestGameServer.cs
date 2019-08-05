using Game.Network;
using GameProto;
using Google.Protobuf;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

public class TestGameServer : MonoBehaviour {

    //测试发包

    public Button BtnLoginTcp;//登陆tcp

    public Button BtnCreateCharacter;//创建角色

    public Button BtnPlayerLevelUp;//玩家升级

    public Button BtnHeroLevelUp;//英雄升级

    public Button BtnUseItem;//使用物品

    void Awake() {
        BtnLoginTcp.onClick.AddListener( LoginTcp );
        BtnCreateCharacter.onClick.AddListener( CreateCharacter );
        BtnPlayerLevelUp.onClick.AddListener( PlayerLevelUp );
        BtnHeroLevelUp.onClick.AddListener( HeroLevelUp );
        BtnUseItem.onClick.AddListener( UseItem );

    }

    void Start() {
        NetworkManager.TCPHostUrl = "127.0.0.1";
        NetworkManager.TCPPort = 50001;

        NetworkManager.Instance.StartConnection();
    }

    private void LoginTcp() {

        JsonData data = new JsonData();
        data["token"] = "123456lls";

        NetworkManager.LoginToken = data;

        NetworkManager.Instance.TcpLogin();

    }


    private void CreateCharacter() {

        Player_Create_Character request = new Player_Create_Character {
            Name = "123456"
        };

        NetworkManager.Instance.DoRequest<Player_Create_Character>( request.ToByteArray(), ( data ) => {
            Player_Create_Character response = Utils.ParseByte<Player_Create_Character>( data );

            Debug.Log( "Player_Create_Character " + response.R );

        } );

    }

    private void PlayerLevelUp() {


    }

    private void HeroLevelUp() {



    }

    private void UseItem() {






    }




}
