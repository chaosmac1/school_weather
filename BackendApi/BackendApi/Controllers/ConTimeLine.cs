#if DEBUG
//#define TEST
#endif

using BackendApi.DataBase;
using BackendApi.DataBase.Type;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace BackendApi.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ConTimeLine : ControllerBase {
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
            InsertLogInDb(GetUserIp());
            
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
            
            // TODO Remove
            Console.WriteLine("Send Array Count:" + timeLineDbs.Count);
            return new ConTimeLineObj.TsTimeLineAll {
                Humidity = ConTimeLineObj.TsTimeLine.FactoryHumidity(timeLineDbs, timezoneOffset),
                Temp = ConTimeLineObj.TsTimeLine.FactoryTemp(timeLineDbs, timezoneOffset),
                WindDirection = ConTimeLineObj.TsChart.FactoryWindDirection(timeLineDbs, timezoneOffset),
                WindSpeed = ConTimeLineObj.TsTimeLine.FactoryWindSpeed(timeLineDbs, timezoneOffset)
            };
        }

        [HttpGet("temp")]
        public ConTimeLineObj.TsTimeLine GetTemp(string? startTime, string? endTime, string? timeValue, int timezoneOffset) {
            InsertLogInDb(GetUserIp());
            
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
            InsertLogInDb(GetUserIp());
            
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
            InsertLogInDb(GetUserIp());
            
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
            InsertLogInDb(GetUserIp());
            
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


        public IPAddress GetUserIp() {
            IPAddress ip;
            var headers = Request.Headers.ToList();
            if (headers.Exists((kvp) => kvp.Key == "X-Forwarded-For"))
            {
                // when running behind a load balancer you can expect this header
                var header = headers.First((kvp) => kvp.Key == "X-Forwarded-For").Value.ToString();
                // in case the IP contains a port, remove ':' and everything after
                ip = IPAddress.Parse(header.Remove(header.IndexOf(':')));
            }
            else
            {
                // this will always have a value (running locally in development won't have the header)
                ip = Request.HttpContext.Connection.RemoteIpAddress?? IPAddress.None;
            }

            return ip;
        }
        
        private void InsertLogInDb(IPAddress ipAddress) {
            try {
                GetCol.GetLog(StaticConf.DbUrl)?.InsertOne(new Log {
                    DateTime = DateTime.UtcNow,
                    IP = ipAddress.ToString()
                });
            }
#if DEBUG
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
#else
            catch (Exception) {  }
#endif
        }
        
        private bool GetTimeLineFromServer(TimeSpan startTime, TimeSpan endTime, ETimeStamp timeValue, out List<TimeLineDb> res) {
            List<TimeLineDb>? fromDb = null;
            if (timeValue == ETimeStamp.FiveSek)
                GetFromServer<TimeLine5Sek, TimeLineDb>(startTime, endTime, out fromDb);
            else if (timeValue == ETimeStamp.OneMin)
                GetFromServer<TimeLine1Min, TimeLineDb>(startTime, endTime, out fromDb);
            else if (timeValue == ETimeStamp.OneH)
                GetFromServer<TimeLine1H, TimeLineDb>(startTime, endTime, out fromDb);
            else if (timeValue == ETimeStamp.OneDay)
                GetFromServer<TimeLine1Day, TimeLineDb>(startTime, endTime, out fromDb);
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
                Builders<T>.Filter.Gte(x => x.CreateTime, tickStart.Ticks) &
                Builders<T>.Filter.Lt(x => x.CreateTime, tickEnd.Ticks),
                new FindOptions<T> {Sort = Builders<T>.Sort.Ascending(x => x.CreateTime)}
            ).ToEnumerable().ToList();
            return true;
        }

        private static bool GetFromServer<T, U>(TimeSpan tickStart, TimeSpan tickEnd, out List<U>? res) where T : TimeLineDb where U : TimeLineDb {
            var db = GetCol.GetColByType<T>(StaticConf.DbUrl);
            res = new List<U>();
            if (db is null) return false;

            var builder = Builders<T>.Filter;
            var findBy = builder.Gte(x => x.CreateTime, tickStart.Ticks) & builder.Lte(x => x.CreateTime, tickEnd.Ticks);
            var sort = new FindOptions<T> {Sort = Builders<T>.Sort.Ascending(x => x.CreateTime)};
            res = (from i in db.FindSync(findBy, sort).ToEnumerable() select i as U).ToList();
            
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