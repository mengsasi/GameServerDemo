using LitJson;
using SQLite4Unity3d;
using System.Collections.Generic;

namespace GameDatabase {

    public class HeroData {
        public string Id;
        public int Level;
    }

    public class ItemData {
        public string Id;
        public int Count;
    }

    [Table( "CharacterData" )]
    public class CharacterData {

        [PrimaryKey, AutoIncrement]
        public int _id { get; set; }

        public string Id { get; set; }//玩家输入的账号

        public string Name { get; set; }//创建角色时的名字

        public int Level { get; set; }

        public string Heros { get; set; }

        public string Items { get; set; }

        public List<ItemData> ListItem = new List<ItemData>();

        public string GetItemJson() {
            JsonData json = new JsonData();
            for( int i = 0; i < ListItem.Count; i++ ) {
                var item = ListItem[i];
                JsonData data = new JsonData();
                data["id"] = item.Id;
                data["count"] = item.Count;
                json.Add( data );
            }
            return json.ToString();
        }

        public List<ItemData> GetItems() {
            List<ItemData> list = new List<ItemData>();
            if( !string.IsNullOrEmpty( Items ) ) {
                try {
                    JsonData data = JsonMapper.ToObject( Items );
                    var l = data.ValueAsArray();
                    foreach( var item in l ) {
                        ItemData ii = new ItemData();
                        ii.Id = item["id"].ValueAsString();
                        ii.Count = item["count"].ValueAsInt();
                        list.Add( ii );
                    }
                }
                catch {
                }
            }
            return list;
        }

    }

}
