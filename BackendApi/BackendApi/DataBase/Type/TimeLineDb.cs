using System;
using System.Linq;
using BackendApi.Ulitis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackendApi.DataBase.Type {
    public abstract class TimeLineDb {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ObjectId _id { get; set; }
        public System.DateTime DateTime { get; set; }
        public float Temp { get; set; }
        public float WindSpeed { get; set; }
        public float Humidity { get; set; }
        public float WindDirection { get; set; }
        public bool DeadValue { get; set; }

        public static U Average<T, U>(DateTime newDateTime, T[] timeLineArr) where T : TimeLineDb, new() where U : TimeLineDb, new() {
            timeLineArr = timeLineArr.Where(x => !x.DeadValue).ToArray();

            return new U {
                DateTime = newDateTime,
                Humidity = timeLineArr.Length == 0 ? 0 : MathF.Round(timeLineArr.Average(x => x.Humidity), 4),
                Temp = timeLineArr.Length == 0 ? 0 : MathF.Round(timeLineArr.Average(x => x.Temp), 4),
                WindDirection = timeLineArr.Length == 0 ? 0 : MathF.Round(timeLineArr.Average(x => x.WindDirection), 4),
                WindSpeed = timeLineArr.Length == 0 ? 0 : MathF.Round(timeLineArr.Average(x => x.WindSpeed), 4),
                DeadValue = timeLineArr.Length == 0
            };
        }
    }

    public class TimeLine5sek : TimeLineDb {
        public static explicit operator TimeLine5sek(Point point) {
            return new TimeLine5sek {
                Humidity = point.Humidity,
                Temp = point.Temp,
                DateTime = new DateTime(point.TimeReal),
                WindDirection = point.WindDirection,
                WindSpeed = point.WindSpeed,
                DeadValue = false
            };
        }
    }
    public class TimeLine1min : TimeLineDb {
        public static explicit operator TimeLine1min(Point point) {
            return new TimeLine1min {
                Humidity = point.Humidity,
                Temp = point.Temp,
                DateTime = new DateTime(point.TimeReal),
                WindDirection = point.WindDirection,
                WindSpeed = point.WindSpeed,
                DeadValue = false
            };
        }
    }
    public class TimeLine1h : TimeLineDb {
        public static explicit operator TimeLine1h(Point point) {
            return new TimeLine1h {
                Humidity = point.Humidity,
                Temp = point.Temp,
                DateTime = new DateTime(point.TimeReal),
                WindDirection = point.WindDirection,
                WindSpeed = point.WindSpeed,
                DeadValue = false
            };
        }
    }
    public class TimeLine1day : TimeLineDb {
        public static explicit operator TimeLine1day(Point point) {
            return new TimeLine1day {
                Humidity = point.Humidity,
                Temp = point.Temp,
                DateTime = new DateTime(point.TimeReal),
                WindDirection = point.WindDirection,
                WindSpeed = point.WindSpeed,
                DeadValue = false
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