using MongoDB.Bson;

namespace BackendApi.DataBase.Type {
    public abstract class TimeLineDb {
        public ObjectId _Id { get; set; }
        public long TimeTicks { get; set; }
        public float Temp  { get; set; }
        public float WindSpeed  { get; set; }
        public float Humidity  { get; set; }
        public float WindDirection  { get; set; }
    }

    public class TimeLine5sek : TimeLineDb { } 
    public class TimeLine1min : TimeLineDb { } 
    public class TimeLine1h : TimeLineDb { } 
    public class TimeLine1day : TimeLineDb { } 
}