using GameDatabase;
using GameProto;
using Google.Protobuf;
using Server;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo {
    public long ID;
    public PlayerDb Instance;
    public NetworkProtoClient Client;
    public string Status;
}

public class PlayerManager {

    private static PlayerManager instance;
    public static PlayerManager Instance {
        get {
            return instance ?? ( instance = new PlayerManager() );
        }
    }

    private Dictionary<long, PlayerInfo> players = new Dictionary<long, PlayerInfo>();

    public PlayerInfo GetPlayer( long id ) {
        if( players.ContainsKey( id ) ) {
            return players[id];
        }
        return null;
    }

    public void Clear( long id, string reason ) {


    }

    public void On_Connect( long id, NetworkProtoClient client ) {
        if( players.ContainsKey( id ) ) {
            Debug.Log( "Already has connect " + id );
            return;
        }
        var info = new PlayerInfo {
            ID = id,
            Instance = null,
            Client = client,
            Status = "login"
        };
        players.Add( id, info );
    }

    public void On_Request( long id, Package_Head head, byte[] data ) {
        var player = GetPlayer( id );
        if( player == null ) {
            return;
        }
        var client = player.Client;
        if( player.Status == "login" ) {
            if( head.Type == "login" ) {
                Login_Process( player, head.Session, data );
            }
            else {
                //状态不正确
                Login login = new Login {
                    R = 1
                };
                byte[] rets = login.ToByteArray();
                client.DoRequest<Login>( rets, head.Session );
            }
        }
        else if( player.Status == "create character" ) {
            if( head.Type == "create_character" ) {
                Create_Character_Process( player, head.Session, data );
            }
            else {
                //状态不正确
                Player_Create_Character pcc = new Player_Create_Character {
                    R = 2
                };
                byte[] rets = pcc.ToByteArray();
                client.DoRequest<Player_Create_Character>( rets, head.Session );
            }
        }
        else {
            Dispose( player, head, data );
        }
    }

    public void Login_Process( PlayerInfo player, long session, byte[] data ) {
        Login login = new Login();
        try {
            Login pkg = Utils.ParseByte<Login>( data );

            //有redis服务器，根据pkg的token去找保存的http登陆的账号数据
            //这里省略
            var id = pkg.Token.Substring( 0, pkg.Token.Length - 3 );

            CharacterData existData = Database.Get<CharacterData>( x => x.Id == id );
            var db = PlayerDb.New( id );
            player.Instance = db;

            if( existData == null ) {
                player.Status = "create character";
            }
            else {
                player.Status = "in game";
            }
            login.R = 0;
            login.Time = Utils.GetTimeStamp();
        }
        catch {
            login.R = 1;
        }
        byte[] rets = login.ToByteArray();
        player.Client.DoRequest<Login>( rets, session );

        CheckInitPlayer( player );
    }

    public void Create_Character_Process( PlayerInfo player, long session, byte[] data ) {
        Player_Create_Character pcc = new Player_Create_Character();
        try {
            Player_Create_Character pkg = Utils.ParseByte<Player_Create_Character>( data );

            var db = player.Instance;
            //创建角色
            int r = db.Create_Character( pkg );
            if( r == 0 ) {
                player.Status = "in game";
            }
            pcc.R = r;
        }
        catch {
            pcc.R = 2;
        }
        byte[] rets = pcc.ToByteArray();
        player.Client.DoRequest<Player_Create_Character>( rets, session );

        CheckInitPlayer( player );
    }

    private void CheckInitPlayer( PlayerInfo player ) {
        //
        if( player.Status == "in game" ) {
            //玩家初始化
        }
    }

    public void Dispose( PlayerInfo player, Package_Head head, byte[] data ) {


    }

    public void Send_Request<T>( long id, byte[] data ) {
        var player = GetPlayer( id );
        if( player == null ) {
            return;
        }
        player.Client.DoRequest<T>( data );
    }

    public void Broadcast<T>( byte[] data ) {
        foreach( var item in players.Values ) {
            item.Client.DoRequest<T>( data );
        }
    }

}
