#if DEBUG
//#define TEST
#endif

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BackendApi.DataBase;
using BackendApi.DataBase.Type;
using BackendApi.ExtensionMethod;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Time = Org.BouncyCastle.Asn1.X509.Time;

namespace BackendApi.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ConTimeLine {
        public class ETimeStamp: Ulitis.FlexEnum<string, ETimeStamp> {
            public static ETimeStamp FiveSek { get; } = new(0, "fiveSek");
            public static ETimeStamp OneMin { get; } = new(1, "oneMin");
            public static ETimeStamp OneH { get; } = new(2, "oneH");
            public static ETimeStamp OneDay { get; } = new(2, "oneDay");
            
            public ETimeStamp(uint id, string value) : base(id, value) { }
        } 
        
        private readonly ILogger<ConTimeLine> _logger;

        public ConTimeLine(ILogger<ConTimeLine> logger) => _logger = logger;
        
        [HttpGet("all")]
        public ConTimeLineObj.TsTimeLineAll GetAll(string? startTime, string? endTime, string? timeValue, int timezoneOffset) {
            if (string.IsNullOrEmpty(startTime)) return new ConTimeLineObj.TsTimeLineAll();
            if (string.IsNullOrEmpty(endTime)) return new ConTimeLineObj.TsTimeLineAll();
            if (string.IsNullOrEmpty(timeValue)) return new ConTimeLineObj.TsTimeLineAll();
            
            var timestamp = ETimeStamp.FiveSek.FindInFlexEnum(timeValue);
            if (timestamp is null) return new ConTimeLineObj.TsTimeLineAll();
            
            var startTimeForDb = Ulitis.DateFixer.ToDbTime(startTime, timezoneOffset);
            var endTimeForDb = Ulitis.DateFixer.ToDbTime(endTime, timezoneOffset);
            
            if (!GetTimeLineFromServer(startTimeForDb, endTimeForDb, timestamp, out var timeLineDbs)) {
                ThrowErr("GetTimeLineFromServer Error");
                return new ConTimeLineObj.TsTimeLineAll();
            }
            
            return new ConTimeLineObj.TsTimeLineAll {
                Humidity = ConTimeLineObj.TsTimeLine.FactoryHumidity(timeLineDbs, timezoneOffset),
                Temp = ConTimeLineObj.TsTimeLine.FactoryTemp(timeLineDbs, timezoneOffset),
                WindDirection = ConTimeLineObj.TsChart.FactoryWindDirection(timeLineDbs, timezoneOffset),
                WindSpeed = ConTimeLineObj.TsTimeLine.FactoryWindSpeed(timeLineDbs, timezoneOffset)
            };
        }

        [HttpGet("temp")]
        public ConTimeLineObj.TsTimeLine GetTemp(string? startTime, string? endTime, string? timeValue, int timezoneOffset) {
            if (string.IsNullOrEmpty(startTime)) return new();
            if (string.IsNullOrEmpty(endTime)) return new();
            if (string.IsNullOrEmpty(timeValue)) return new();
            
            var timestamp = ETimeStamp.FiveSek.FindInFlexEnum(timeValue);
            if (timestamp is null) return new();
            
            var startTimeForDb = Ulitis.DateFixer.ToDbTime(startTime, timezoneOffset);
            var endTimeForDb = Ulitis.DateFixer.ToDbTime(endTime, timezoneOffset);
            
            if (!GetTimeLineFromServer(startTimeForDb, endTimeForDb, timestamp, out var timeLineDbs)) {
                ThrowErr("GetTimeLineFromServer Error");
                return new ConTimeLineObj.TsTimeLine();
            }

            return ConTimeLineObj.TsTimeLine.FactoryTemp(timeLineDbs, timezoneOffset);
        }
        
        [HttpGet("windspeed")]
        public ConTimeLineObj.TsTimeLine GetWindSpeed(string? startTime, string? endTime, string? timeValue, int timezoneOffset) {
            if (string.IsNullOrEmpty(startTime)) return new();
            if (string.IsNullOrEmpty(endTime)) return new();
            if (string.IsNullOrEmpty(timeValue)) return new();
            
            var timestamp = ETimeStamp.FiveSek.FindInFlexEnum(timeValue);
            if (timestamp is null) return new();
            
            var startTimeForDb = Ulitis.DateFixer.ToDbTime(startTime, timezoneOffset);
            var endTimeForDb = Ulitis.DateFixer.ToDbTime(endTime, timezoneOffset);
        
            if (!GetTimeLineFromServer(startTimeForDb, endTimeForDb, timestamp, out var timeLineDbs)) {
                ThrowErr("GetTimeLineFromServer Error");
                return new ConTimeLineObj.TsTimeLine();
            }
            
            return ConTimeLineObj.TsTimeLine.FactoryWindSpeed(timeLineDbs, timezoneOffset);
        }
        
        [HttpGet("humidity")]
        public ConTimeLineObj.TsTimeLine GetHumidity(string? startTime, string? endTime, string timeValue, int timezoneOffset) {
            if (string.IsNullOrEmpty(startTime)) return new();
            if (string.IsNullOrEmpty(endTime)) return new();
            if (string.IsNullOrEmpty(timeValue)) return new();
            
            var timestamp = ETimeStamp.FiveSek.FindInFlexEnum(timeValue);
            if (timestamp is null) return new();
            
            var startTimeForDb = Ulitis.DateFixer.ToDbTime(startTime, timezoneOffset);
            var endTimeForDb = Ulitis.DateFixer.ToDbTime(endTime, timezoneOffset);
        
            if (!GetTimeLineFromServer(startTimeForDb, endTimeForDb, timestamp, out var timeLineDbs)) {
                ThrowErr("GetTimeLineFromServer Error");
                return new ConTimeLineObj.TsTimeLine();
            }

            return ConTimeLineObj.TsTimeLine.FactoryHumidity(timeLineDbs, timezoneOffset);
        }
        
        [HttpGet("windDirection")]
        public ConTimeLineObj.TsTimeLine GetWindDirection(string? startTime, string? endTime, string? timeValue, int timezoneOffset) {
            if (string.IsNullOrEmpty(startTime)) return new();
            if (string.IsNullOrEmpty(endTime)) return new();
            if (string.IsNullOrEmpty(timeValue)) return new();
            
            var timestamp = ETimeStamp.FiveSek.FindInFlexEnum(timeValue);
            if (timestamp is null) return new();
            
            var startTimeForDb = Ulitis.DateFixer.ToDbTime(startTime, timezoneOffset);
            var endTimeForDb = Ulitis.DateFixer.ToDbTime(endTime, timezoneOffset);
        
            if (!GetTimeLineFromServer(startTimeForDb, endTimeForDb, timestamp, out var timeLineDbs)) {
                ThrowErr("GetTimeLineFromServer Error");
                return new ConTimeLineObj.TsTimeLine();
            }

            return ConTimeLineObj.TsTimeLine.FactoryWindSpeed(timeLineDbs, timezoneOffset);
        }


        private bool GetTimeLineFromServer(TimeSpan startTime, TimeSpan endTime, ETimeStamp timeValue, out List<TimeLineDb> res) {
            List<TimeLineDb>? fromDb = null;
            if (timeValue == ETimeStamp.FiveSek)
                GetFromServer<TimeLine5sek, TimeLineDb>(startTime, endTime, out fromDb);
            else if (timeValue == ETimeStamp.OneMin)
                GetFromServer<TimeLine1min, TimeLineDb>(startTime, endTime, out fromDb);
            else if (timeValue == ETimeStamp.OneH)
                GetFromServer<TimeLine1h, TimeLineDb>(startTime, endTime, out fromDb);
            else if (timeValue == ETimeStamp.OneDay)
                GetFromServer<TimeLine1day, TimeLineDb>(startTime, endTime, out fromDb);
            else {
                res = new List<TimeLineDb>();
                return ThrowErr("timeValue Enum Type Not Found");
            }
            
            if (fromDb is null) {
                res = new List<TimeLineDb>();
                return ThrowErr("GetFromServer Error");
            }
            
            res = fromDb;
            return true;
        }

        private static bool GetFromServer<T>(TimeSpan tickStart, TimeSpan tickEnd, out List<T>? res) where T : TimeLineDb {
            var db = GetCol.GetColByType<T>(StaticConf.DbUrl);
            res = new List<T>();
            if (db is null) return false;

            res = db.FindSync(
                Builders<T>.Filter.Gte(x => x.CreateTime, new DateTime(tickStart.Ticks, DateTimeKind.Utc)) &
                Builders<T>.Filter.Lt(x => x.CreateTime, new DateTime(tickEnd.Ticks, DateTimeKind.Utc)),
                new FindOptions<T> {Sort = Builders<T>.Sort.Ascending(x => x.CreateTime)}
            ).ToEnumerable().ToList();
            return true;
        }

        private static bool GetFromServer<T, U>(TimeSpan tickStart, TimeSpan tickEnd, out List<U>? res) where T : TimeLineDb where U : TimeLineDb {
            var db = GetCol.GetColByType<T>(StaticConf.DbUrl);
            res = new List<U>();
            if (db is null) return false;

            var start = new DateTime(tickStart.Ticks, DateTimeKind.Utc);
            var end = new DateTime(tickEnd.Ticks, DateTimeKind.Utc);
            res = db.FindSync(
                Builders<T>.Filter.Gte(x => x.CreateTime, new DateTime(tickStart.Ticks, DateTimeKind.Utc)) &
                Builders<T>.Filter.Lt(x => x.CreateTime, new DateTime(tickEnd.Ticks, DateTimeKind.Utc)),
                new FindOptions<T> {Sort = Builders<T>.Sort.Ascending(x => x.CreateTime)}
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
    }
}