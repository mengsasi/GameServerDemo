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
            //byte[] rets = login.ToByteArray();
            //player.Client.DoRequest<Login>( rets, session );
            CheckInitPlayer( player );
            return login.ToByteArray();
        }

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
            //byte[] rets = pcc.ToByteArray();
            //player.Client.DoRequest<Player_Create_Character>( rets, session );
            CheckInitPlayer( player );
            return pcc.ToByteArray();
        }

        private void CheckInitPlayer( PlayerInfo player ) {
            //
            if( player.Status == "in game" ) {
                //玩家初始化



            }
        }

        public byte[] Dispose( PlayerInfo player, Package_Head head, byte[] data ) {
            if( head.Type == Utils.GetProtpType<Heartbeat>() ) {
                if( player.Instance != null ) {
                    player.Instance.Heartbeat();
                }
                return null;
            }

            return new byte[0];
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

}
