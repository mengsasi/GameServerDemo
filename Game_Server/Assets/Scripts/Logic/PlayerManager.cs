using GameDatabase;
using GameProto;
using Google.Protobuf;
using Server;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Logic {

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

        public void Clear( long id ) {
            var player = GetPlayer( id );
            if( player != null ) {
                player.Client.Close();
                if( player.Instance != null ) {
                    player.Instance.Exit();
                }
                players.Remove( id );
            }
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

        //错误值r=2，是流程错误
        public void On_Request( long id, Package_Head head, byte[] data ) {
            var player = GetPlayer( id );
            if( player == null ) {
                return;
            }
            var client = player.Client;
            var headType = head.Type;
            byte[] rets = null;

            if( player.Status == "login" ) {
                if( headType == Utils.GetProtpType<Login>() ) {
                    rets = Login_Process( player, head.Session, data );
                }
                else {
                    Login login = new Login {
                        R = 2
                    };
                    rets = login.ToByteArray();
                }
            }
            else if( player.Status == "create character" ) {
                if( headType == Utils.GetProtpType<Player_Create_Character>() ) {
                    rets = Create_Character_Process( player, head.Session, data );
                }
                else {
                    //流程错误
                    Player_Create_Character pcc = new Player_Create_Character {
                        R = 2
                    };
                    rets = pcc.ToByteArray();
                }
            }
            else {
                rets = Dispose( player, head, data );
            }

            if( rets != null ) {
                MethodInfo doRequestMethod = player.Client.GetType().GetMethod( "DoRequest" );
                Type tt = Type.GetType( headType );
                doRequestMethod = doRequestMethod.MakeGenericMethod( tt );
                var arguments = new object[]
                {
                    rets,
                    head.Session
                };

                doRequestMethod.Invoke( player.Client, arguments );
            }
        }

        //处理登陆包
        public byte[] Login_Process( PlayerInfo player, long session, byte[] data ) {
            Login login = new Login();
            try {
                Login pkg = Utils.ParseByte<Login>( data );

                //有redis服务器，根据pkg的token去找保存的http登陆的账号数据
                //这里省略
                var id = pkg.Token.Substring( 0, pkg.Token.Length - 3 );

                CharacterData existData = Database.Get<CharacterData>( x => x.Id == id );
                var db = PlayerDb.New( id );
                player.Instance = db;

                if( existData == null || string.IsNullOrEmpty( existData.Name ) ) {
                    player.Status = "create character";
                }
                else {
                    player.Status = "in game";
                }
                login.R = 1;
                login.Time = Utils.GetTimeStamp();
            }
            catch {
                login.R = 2;
            }
            CheckInitPlayer( player );
            return login.ToByteArray();
        }

        //处理创建角色包
        public byte[] Create_Character_Process( PlayerInfo player, long session, byte[] data ) {
            Player_Create_Character pcc = new Player_Create_Character();
            try {
                Player_Create_Character pkg = Utils.ParseByte<Player_Create_Character>( data );

                var db = player.Instance;
                //创建角色
                int r = db.Create_Character( pkg );
                if( r == 1 ) {
                    player.Status = "in game";
                }
                pcc.R = r;
            }
            catch {
                pcc.R = 2;
            }
            CheckInitPlayer( player );
            return pcc.ToByteArray();
        }

        //是否初始化玩家
        private void CheckInitPlayer( PlayerInfo player ) {
            if( player.Status == "in game" ) {
                //玩家初始化
                Register( player.Instance.Id, player );
                player.Instance.Init();
            }
        }

        //处理一般包
        public byte[] Dispose( PlayerInfo player, Package_Head head, byte[] data ) {
            if( player.Status != "in game" ) {
                return null;
            }
            string headType = head.Type;
            if( headType == Utils.GetProtpType<Heartbeat>() ) {
                if( player.Instance != null ) {
                    player.Instance.Heartbeat();
                }
                return null;
            }
            var pack = Utils.GetPackName( headType );
            var first_index = pack.IndexOf( '_' );
            var process = pack.Substring( 0, first_index );
            var methodname = pack.Substring( first_index + 1 );

            var processor = "Logic." + process + "Processor";
            Type typeProcessor = Type.GetType( processor );
            MethodInfo method = typeProcessor.GetMethod( methodname, BindingFlags.Static | BindingFlags.Public );
            var arguments = new object[]
            {
                player.Instance,
                data
            };
            player.Instance.Save();
            return (byte[])method.Invoke( null, arguments );
        }

        private Dictionary<string, PlayerInfo> playerIds = new Dictionary<string, PlayerInfo>();

        public void Register( string id, PlayerInfo playerinfo ) {
            if( !playerIds.ContainsKey( id ) ) {
                playerIds.Add( id, playerinfo );
            }
        }

        public void UnRegister( string id ) {
            if( playerIds.ContainsKey( id ) ) {
                playerIds.Remove( id );
            }
        }

        public PlayerInfo GetPlayerById( string id ) {
            if( playerIds.ContainsKey( id ) ) {
                return playerIds[id];
            }
            return null;
        }

        public void Send_Request<T>( string id, byte[] data ) {
            var player = GetPlayerById( id );
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

}
