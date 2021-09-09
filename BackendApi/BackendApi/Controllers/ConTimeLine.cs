using System;
using System.Collections.Generic;
using System.Linq;
using BackendApi.DataBase;
using BackendApi.DataBase.Type;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace BackendApi.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ConTimeLine {
        public enum ETimeValue {
            FiveSek,
            OneMin,
            OneH,
            OneDay
        }
        private readonly ILogger<ConTimeLine> _logger;

        public ConTimeLine(ILogger<ConTimeLine> logger) {
            _logger = logger;
        }

        [HttpGet("all")]
        public TsTimeLineAll GetAll(long startTime, long endTime, string timeValue) {
            if (!string.IsNullOrEmpty(timeValue)) return new TsTimeLineAll();

            if (!GetTimeLineFromServer(startTime, endTime, timeValue, out var timeLineDbs)) {
                ThrowErr("GetTimeLineFromServer Error");
                return new TsTimeLineAll();
            }

            return new TsTimeLineAll {
                Humidity = new TsTimeLine {Points = (from i in timeLineDbs select new TsTimeLine.Point(i.TimeTicks, i.Humidity)).ToArray()},
                Temp = new TsTimeLine {Points = (from i in timeLineDbs select new TsTimeLine.Point(i.TimeTicks, i.Temp)).ToArray()},
                WindDirection = new TsTimeLine {Points = (from i in timeLineDbs select new TsTimeLine.Point(i.TimeTicks, i.WindDirection)).ToArray()},
                WindSpeed = new TsTimeLine {Points = (from i in timeLineDbs select new TsTimeLine.Point(i.TimeTicks, i.WindSpeed)).ToArray()}
            };
        }

        [HttpGet("temp")]
        public TsTimeLine GetTemp(long startTime, long endTime, string timeValue) {
            if (!string.IsNullOrEmpty(timeValue)) return new TsTimeLine();

            if (!GetTimeLineFromServer(startTime, endTime, timeValue, out var timeLineDbs)) {
                ThrowErr("GetTimeLineFromServer Error");
                return new TsTimeLine();
            }

            return new TsTimeLine {
                Points = (from i in timeLineDbs select new TsTimeLine.Point(i.TimeTicks, i.Temp)).ToArray()
            };
        }

        [HttpGet("windspeed")]
        public TsTimeLine GetWindSpeed(long startTime, long endTime, string timeValue) {
            if (!string.IsNullOrEmpty(timeValue)) return new TsTimeLine();

            if (!GetTimeLineFromServer(startTime, endTime, timeValue, out var timeLineDbs)) {
                ThrowErr("GetTimeLineFromServer Error");
                return new TsTimeLine();
            }

            return new TsTimeLine {
                Points = (from i in timeLineDbs select new TsTimeLine.Point(i.TimeTicks, i.WindSpeed)).ToArray()
            };
        }

        [HttpGet("humidity")]
        public TsTimeLine GetHumidity(long startTime, long endTime, string timeValue) {
            if (!string.IsNullOrEmpty(timeValue)) return new TsTimeLine();

            if (!GetTimeLineFromServer(startTime, endTime, timeValue, out var timeLineDbs)) {
                ThrowErr("GetTimeLineFromServer Error");
                return new TsTimeLine();
            }

            return new TsTimeLine {
                Points = (from i in timeLineDbs select new TsTimeLine.Point(i.TimeTicks, i.Humidity)).ToArray()
            };
        }



        [HttpGet("windDirection")]
        public TsTimeLine GetWindDirection(long startTime, long endTime, string timeValue) {
            if (!string.IsNullOrEmpty(timeValue)) return new TsTimeLine();

            if (!GetTimeLineFromServer(startTime, endTime, timeValue, out var timeLineDbs)) {
                ThrowErr("GetTimeLineFromServer Error");
                return new TsTimeLine();
            }

            return new TsTimeLine {
                Points = (from i in timeLineDbs select new TsTimeLine.Point(i.TimeTicks, i.WindDirection)).ToArray()
            };
        }


        private bool GetTimeLineFromServer(long startTime, long endTime, string timeValue, out List<TimeLineDb> res) {
            var timeValueFound = false;
            ETimeValue eTimeValue = 0;
            foreach (var i in Enum.GetValues<ETimeValue>()) {
                if (i.ToString() != timeValue) continue;
                timeValueFound = true;
                eTimeValue = i;
                break;
            }

            if (timeValueFound == false) {
                res = new List<TimeLineDb>();
                return ThrowErr("timeValue Enum Type Not Found");
            }

            List<TimeLineDb>? fromDb = null;
            if (eTimeValue switch {
                ETimeValue.FiveSek =>
                    GetFromServer<TimeLine5sek, TimeLineDb>(startTime, endTime, out fromDb),
                ETimeValue.OneMin =>
                    GetFromServer<TimeLine1min, TimeLineDb>(startTime, endTime, out fromDb),
                ETimeValue.OneH =>
                    GetFromServer<TimeLine1h, TimeLineDb>(startTime, endTime, out fromDb),
                ETimeValue.OneDay =>
                    GetFromServer<TimeLine1day, TimeLineDb>(startTime, endTime, out fromDb),
                _ => false
            } == false || fromDb is null) {
                res = new List<TimeLineDb>();
                return ThrowErr("GetFromServer Error");
            }

            res = fromDb;
            return true;
        }

        private static bool GetFromServer<T>(long tickStart, long tickEnd, out List<T>? res) where T : TimeLineDb {
            var db = GetCol.GetColByType<T>(StaticConf.DbUrl);
            res = new List<T>();
            if (db is null) return false;

            res = db.FindSync(
                Builders<T>.Filter.Gte(x => x.TimeTicks, tickStart) &
                Builders<T>.Filter.Lt(x => x.TimeTicks, tickEnd),
                new FindOptions<T> {Sort = Builders<T>.Sort.Ascending(x => x.TimeTicks)}
            ).ToEnumerable().ToList();
            return true;
        }

        private static bool GetFromServer<T, U>(long tickStart, long tickEnd, out List<U>? res) where T : TimeLineDb where U : TimeLineDb {
            var db = GetCol.GetColByType<T>(StaticConf.DbUrl);
            res = new List<U>();
            if (db is null) return false;

            res = db.FindSync(
                Builders<T>.Filter.Gte(x => x.TimeTicks, tickStart) &
                Builders<T>.Filter.Lt(x => x.TimeTicks, tickEnd),
                new FindOptions<T> {Sort = Builders<T>.Sort.Ascending(x => x.TimeTicks)}
            ).ToEnumerable().ToList() as List<U>;
            return true;
        }

        private static bool ThrowErr(string msg) {
#if DEBUG
            throw new Exception(msg);
#else
            return false;
#endif
        }

        public class TsTimeLine {
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            public Point[]? Points { get; set; }

            public class Point {

                public Point(long pointId, float value) {
                    PointId = pointId;
                    Value = value;
                }
                
                // ReSharper disable once UnusedAutoPropertyAccessor.Global
                public long PointId { get; set; }
                // ReSharper disable once UnusedAutoPropertyAccessor.Global
                public float Value { get; set; }
            }
        }
        public class TsTimeLineAll {
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            public TsTimeLine? Temp { get; set; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            public TsTimeLine? WindSpeed { get; set; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            public TsTimeLine? Humidity { get; set; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            public TsTimeLine? WindDirection { get; set; }
        }
    }
}