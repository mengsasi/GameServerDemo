using LitJson;
using SQLite4Unity3d;
using System.Collections.Generic;

public class Item {
    public string Id;
    public int Count;
}

[Table( "CharacterData" )]
public class CharacterData {

    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    public string UserCode { get; set; }

    public string Name { get; set; }

    public int Level { get; set; }

    public string Items { get; set; }

    public List<Item> ListItem = new List<Item>();

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

    public List<Item> GetItems() {
        List<Item> list = new List<Item>();
        if( !string.IsNullOrEmpty( Items ) ) {
            try {
                JsonData data = JsonMapper.ToObject( Items );
                var l = data.ValueAsArray();
                foreach( var item in l ) {
                    Item ii = new Item();
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

