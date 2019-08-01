using GameDatabase;
using GameProto;
using Google.Protobuf;
using System;
using System.IO;
using UnityEngine;

public class TestSproto : MonoBehaviour {

    void Start() {

        Character character = new Character();
        character.Id = "1";
        character.Name = "aaa";

        byte[] datas = character.ToByteArray();


        Type t = Type.GetType( "Character" );



        IMessage imCharacter = new Character();
        Character ch1 = new Character();
        ch1 = (Character)imCharacter.Descriptor.Parser.ParseFrom( datas );




        Debug.Log( ch1.Id );
        Debug.Log( ch1.Name );





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
