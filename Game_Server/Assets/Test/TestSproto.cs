using GameDatabase;
using GameProto;
using Google.Protobuf;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

public class TestSproto : MonoBehaviour {

    void Start() {

        Character character = new Character();
        character.Id = "1";
        character.Name = "aaa";

        byte[] datas = character.ToByteArray();

        //IMessage imCharacter = new Character();
        //Character ch1 = new Character();
        //ch1 = (Character)imCharacter.Descriptor.Parser.ParseFrom( datas );


        string name = "Global_Use_Item";

        byte[] ddd = System.Text.Encoding.UTF8.GetBytes( name );


        string nn = System.Text.Encoding.UTF8.GetString( ddd );

        Debug.Log( nn );


        string db_func = "global_use_item";


        var indexOf_ = db_func.IndexOf( '_' );
        string db = db_func.Substring( 0, indexOf_ );
        string func = db_func.Substring( indexOf_ + 1 );

        Debug.Log( db );
        Debug.Log( func );




        //Debug.Log( ch1.Id );
        //Debug.Log( ch1.Name );





        //CharacterData data = new CharacterData();
        //data._id = 11;
        //data.Id = "11";

        //data.Name = "222222";



        //try {
        //    var d = Database.Get<CharacterData>( x => x._id == 11 );
        //    if( d == null ) {

        //        Database.Insert( data );

        //    }
        //    else {

        //        Database.Update( data );

        //    }
        //}
        //catch {




        //}



        Database.Close();








    }


}
