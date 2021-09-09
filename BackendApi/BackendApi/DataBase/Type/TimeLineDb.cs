using System.Collections.Generic;
using BackendApi.Ulitis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackendApi.DataBase.Type {
    public abstract class TimeLineDb {
        public ObjectId _Id { get; set; }
        public long TimeTicks { get; set; }
        public float Temp  { get; set; }
        public float WindSpeed  { get; set; }
        public float Humidity  { get; set; }
        public float WindDirection  { get; set; }
    }

    public class TimeLine5sek : TimeLineDb {
        public static explicit operator TimeLine5sek(Point point) {
            return new() {
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
            return new() {
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
            return new() {
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
            return new() {
                Humidity = point.Humidity,
                Temp = point.Temp,
                TimeTicks = point.TimeReal,
                WindDirection = point.WindDirection,
                WindSpeed = point.WindSpeed
            };
        }
    }

    public class Acc {
        [BsonId] 
        public ObjectId _Id { get; set; }
        public string UsernameHash { get; set; }
        public string PasswdHash { get; set; }
    }
}