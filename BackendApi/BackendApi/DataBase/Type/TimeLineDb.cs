using BackendApi.Ulitis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackendApi.DataBase.Type {
    public abstract class TimeLineDb {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ObjectId _Id { get; set; }
        public long TimeTicks { get; set; }
        public float Temp { get; set; }
        public float WindSpeed { get; set; }
        public float Humidity { get; set; }
        public float WindDirection { get; set; }
    }

    public class TimeLine5sek : TimeLineDb {
        public static explicit operator TimeLine5sek(Point point) {
            return new TimeLine5sek {
                Humidity = point.Humidity,
                Temp = point.Temp,
                TimeTicks = point.TimeReal,
                WindDirection = point.WindDirection,
                WindSpeed = point.WindSpeed
            };
        }
    }
    public class TimeLine1min : TimeLineDb {
        public static explicit operator TimeLine1min(Point point) {
            return new TimeLine1min {
                Humidity = point.Humidity,
                Temp = point.Temp,
                TimeTicks = point.TimeReal,
                WindDirection = point.WindDirection,
                WindSpeed = point.WindSpeed
            };
        }
    }
    public class TimeLine1h : TimeLineDb {
        public static explicit operator TimeLine1h(Point point) {
            return new TimeLine1h {
                Humidity = point.Humidity,
                Temp = point.Temp,
                TimeTicks = point.TimeReal,
                WindDirection = point.WindDirection,
                WindSpeed = point.WindSpeed
            };
        }
    }
    public class TimeLine1day : TimeLineDb {
        public static explicit operator TimeLine1day(Point point) {
            return new TimeLine1day {
                Humidity = point.Humidity,
                Temp = point.Temp,
                TimeTicks = point.TimeReal,
                WindDirection = point.WindDirection,
                WindSpeed = point.WindSpeed
            };
        }
    }

    public class Acc {
        // ReSharper disable once InconsistentNaming
        [BsonId] public ObjectId _Id { get; set; }
        public string? UsernameHash { get; set; }
        public string? PasswdHash { get; set; }
    }
}