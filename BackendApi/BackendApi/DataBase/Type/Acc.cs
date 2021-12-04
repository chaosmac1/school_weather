using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackendApi.DataBase.Type;

public class Acc {
    // ReSharper disable once InconsistentNaming
    [BsonId] public ObjectId _id { get; set; }
    public string? UsernameHash { get; set; }
    public string? PasswdHash { get; set; }
}