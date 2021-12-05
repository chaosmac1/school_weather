using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackendApi.DataBase.Type; 

public abstract class TimeLineDb {
    [BsonId]
    public ObjectId _id { get; set; }

    [BsonElement]
    private DateTime _createTime; 
    public long CreateTime { get => _createTime.Ticks; set => _createTime = new DateTime(value, DateTimeKind.Utc); }
    [BsonElement]
    public float Temp { get; set; }
    [BsonElement]
    public float WindSpeed { get; set; }
    [BsonElement]
    public float Humidity { get; set; }
    [BsonElement]
    public float WindDirection { get; set; }
    [BsonElement]
    public bool DeadValue { get; set; }

    public static U Average<T, U>(TimeSpan createTime, T[] timeLineArr) where T : TimeLineDb, new() where U : TimeLineDb, new() {
        timeLineArr = timeLineArr.Where(x => !x.DeadValue).ToArray();

        return new U {
            CreateTime = createTime.Ticks,
            Humidity = timeLineArr.Length == 0 ? 0 : MathF.Round(timeLineArr.Average(x => x.Humidity), 4),
            Temp = timeLineArr.Length == 0 ? 0 : MathF.Round(timeLineArr.Average(x => x.Temp), 4),
            WindDirection = timeLineArr.Length == 0 ? 0 : MathF.Round(timeLineArr.Average(x => x.WindDirection), 4),
            WindSpeed = timeLineArr.Length == 0 ? 0 : MathF.Round(timeLineArr.Average(x => x.WindSpeed), 4),
            DeadValue = timeLineArr.Length == 0
        };
    }
}