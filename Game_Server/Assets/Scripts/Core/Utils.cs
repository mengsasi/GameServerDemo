using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Utils {

    public static byte[] String2Byte( string str ) {
        return System.Text.Encoding.UTF8.GetBytes( str );
    }

    public static string Byte2String( byte[] bytes ) {
        return System.Text.Encoding.UTF8.GetString( bytes );
    }



    public static void Call( string db_func, string playerId, byte[] data ) {
        if( !string.IsNullOrEmpty( db_func ) ) {
            var indexOf_ = db_func.IndexOf( '_' );
            string db = db_func.Substring( 0, indexOf_ );
            string func = db_func.Substring( indexOf_ + 1 );

            string dbdb = db + "Db";
            Type type = Type.GetType( dbdb );
            MethodInfo meth = type.GetMethod( "func" );


            //meth.Invoke()

        }

    }


    public static long GetTimeStamp() {
        var utcNow = DateTime.UtcNow;
        var timeSpan = utcNow - new DateTime( 1970, 1, 1, 0, 0, 0 );
        return (long)timeSpan.TotalSeconds;
    }



}
