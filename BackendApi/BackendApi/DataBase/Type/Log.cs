using MongoDB.Bson.Serialization.Attributes;

namespace BackendApi.DataBase.Type; 

public class Log {
    [BsonId] 
    public Guid _id { get; set; }
    public string IP { get; set; }
    public DateTime DateTime { get; set; }
}