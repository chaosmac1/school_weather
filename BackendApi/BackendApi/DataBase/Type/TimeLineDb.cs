using MongoDB.Bson;

namespace BackendApi.DataBase.Type; 

public abstract class TimeLineDb {
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public ObjectId _id { get; set; }
    public DateTime CreateTime { get; set; }
    public float Temp { get; set; }
    public float WindSpeed { get; set; }
    public float Humidity { get; set; }
    public float WindDirection { get; set; }
    public bool DeadValue { get; set; }

    public static U Average<T, U>(DateTime newDateTime, T[] timeLineArr) where T : TimeLineDb, new() where U : TimeLineDb, new() {
        timeLineArr = timeLineArr.Where(x => !x.DeadValue).ToArray();

        return new U {
            CreateTime = newDateTime,
            Humidity = timeLineArr.Length == 0 ? 0 : MathF.Round(timeLineArr.Average(x => x.Humidity), 4),
            Temp = timeLineArr.Length == 0 ? 0 : MathF.Round(timeLineArr.Average(x => x.Temp), 4),
            WindDirection = timeLineArr.Length == 0 ? 0 : MathF.Round(timeLineArr.Average(x => x.WindDirection), 4),
            WindSpeed = timeLineArr.Length == 0 ? 0 : MathF.Round(timeLineArr.Average(x => x.WindSpeed), 4),
            DeadValue = timeLineArr.Length == 0
        };
    }
}