﻿using Config;
using GameDatabase;
using GameProto;
using Google.Protobuf;
using LitJson;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

public class TestSproto : MonoBehaviour {


    void Start() {

        //Character character = new Character();
        //character.Id = "1";
        //character.Name = "aaa";

        //byte[] datas = character.ToByteArray();

        //IMessage imCharacter = new Character();
        //Character ch1 = new Character();
        //ch1 = (Character)imCharacter.Descriptor.Parser.ParseFrom( datas );

        //Character ch2 = Utils.ParseByte<Character>( datas );

        //Debug.Log( ch2.Name );


        //string name = "Global_Use_Item";

        //byte[] ddd = System.Text.Encoding.UTF8.GetBytes( name );

        //string nn = System.Text.Encoding.UTF8.GetString( ddd );

        //Debug.Log( nn );


        //string db_func = "global_use_item";

        //var indexOf_ = db_func.IndexOf( '_' );
        //string db = db_func.Substring( 0, indexOf_ );
        //string func = db_func.Substring( indexOf_ + 1 );

        //Debug.Log( db );
        //Debug.Log( func );


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


        //CharacterData existData = Database.Get<CharacterData>( x => x.Id == "1" );
        //Debug.Log( existData == null );


        //CharacterData data = new CharacterData() {
        //    Id = "1"
        //};
        //Database.Insert( data );


        //string sstr = "tokenlls";
        //var substr = sstr.Substring( 0, sstr.Length - 3 );
        //Debug.Log( substr );

        //var data = Database.Get<CharacterData>( x => x.Id == "111" );
        //Debug.Log( data == null );
        //Database.Close();



        //var type = Utils.GetProtpType<Login>();
        //var str = Utils.GetPackName( type );
        //Debug.Log( str );



        //JsonData heros = new JsonData();
        //for( int i = 0; i < 3; i++ ) {
        //    JsonData hero = new JsonData {
        //        ["Id"] = ( i + 1 ).ToString(),
        //        ["Level"] = 1
        //    };
        //    heros.Add( hero );
        //}
        //Debug.Log( heros.ToJson() );


        //var conf = LevelConfigs.Instance.GetConf( 1 );
        //Debug.Log( conf.Level );



        //var pack = "Global_Use_Item";
        //var first_index = pack.IndexOf( '_' );
        //var process = pack.Substring( 0, first_index );
        //var methodname = pack.Substring( first_index + 1 );

        //Debug.Log( process );
        //Debug.Log( methodname );


        //反射获取静态方法
        //byte[] bb = Utils.String2Byte( "11" );
        //Debug.Log( "BBBBB " + Utils.Byte2String( bb ) );

        //Type typeProcessor = Type.GetType( "TestSproto" );
        //MethodInfo method = typeProcessor.GetMethod( "GetPack", BindingFlags.Static | BindingFlags.Public );
        //var arguments = new object[]
        //{
        //    bb
        //};
        //var print = (byte[])method.Invoke( null, arguments );

        //Debug.Log( "print " + Utils.Byte2String( print ) );



        //var data = Utils.String2Byte( "111" );

        //var processor = "Logic." + "Hero" + "Processor";
        //Type typeProcessor = Type.GetType( processor );
        //MethodInfo method = typeProcessor.GetMethod( "Test", BindingFlags.Static | BindingFlags.Public );
        //var arguments = new object[]
        //{
        //    data
        //};
        //var tt = (byte[])method.Invoke( null, arguments );

        //var dd = Utils.Byte2String( tt );

        //Debug.Log( dd );






    }

    public static byte[] GetPack( byte[] arg ) {
        return arg;
    }

}
