using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using UnityEngine;

namespace GameDatabase {

    public class Database {

        public const string DbName = "database.db";

        private static SQLiteConnection connection;
        private static SQLiteConnection Connection {
            get {
                if( connection == null ) {
#if UNITY_EDITOR
                    var path = Application.streamingAssetsPath + "/" + DbName;
#else 
                    var path = Application.persistentDataPath + "/" + DbName;
#endif
                    try {
                        connection = new SQLiteConnection( path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.PrivateCache ) {
                            TimeExecution = true
                        };
                        connection.Execute( "PRAGMA synchronous = OFF" );
                        connection.TimeExecutionEvent += _connection_TimeExecutionEvent;
                        CreateAllTable();
                    }
                    catch( SQLiteException se ) {
                        if( se.Result != SQLite3.Result.OK ) {
                            connection.Close();
                            connection = null;
                            File.Delete( path );
                        }
                    }
                }
                return connection;
            }
        }

        private static void _connection_TimeExecutionEvent( TimeSpan executionTime, TimeSpan totalExecutionTime ) {
            if( executionTime.Milliseconds > 5 ) {
                Debug.LogWarning( "数据库操作超时 : " + executionTime.Milliseconds );
            }
        }

        /// <summary>
        /// 创建表
        /// </summary>
        private static void CreateAllTable() {
            //To Create
            Connection.CreateTable<CharacterData>();
        }

        public static void Execute( string query, params object[] args ) {
            Connection.Execute( query, args );
        }

        public static TableQuery<T> GetTable<T>() where T : new() {
            return Connection.Table<T>();
        }

        public static void Insert( object obj ) {
            Connection.Insert( obj );
        }

        public static void Update( object obj ) {
            Connection.Update( obj );
        }

        /// <summary>
        /// 删除数据库中的指定项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">该项中的主键</param>
        public static void Delete<T>( object obj ) {
            Connection.Delete<T>( obj );
        }

        public static void Close() {
            if( connection != null ) {
                connection.Close();
                connection = null;
            }
        }

        /// <summary>
        /// 获取，一个条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T Get<T>( Expression<Func<T, bool>> predicate ) where T : new() {
            return Connection.Get<T>( predicate );
        }

        /// <summary>
        /// 获取，两个条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate1">首要查找条件，UserId</param>
        /// <param name="predicate2">次要查找条件</param>
        /// <returns></returns>
        public static T Get<T>( Expression<Func<T, bool>> predicate1, Expression<Func<T, bool>> predicate2 ) where T : new() {
            TableQuery<T> table = Connection.Table<T>().Where( predicate1 );
            if( table != null ) {
                return table.Where( predicate2 ).First();
            }
            return default( T );
        }

        public static List<T> GetAll<T>( Expression<Func<T, bool>> predicate ) where T : new() {
            try {
                TableQuery<T> table = Connection.Table<T>().Where( predicate );
                List<T> list = new List<T>();
                foreach( var item in table ) {
                    list.Add( item );
                }
                return list;
            }
            catch {
                return null;
            }
        }
    }
}