using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BackendApi.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ConTimeLine {
        private readonly ILogger<ConTimeLine> _logger;

        public ConTimeLine(ILogger<ConTimeLine> logger) => _logger = logger;

        public class TsTimeLine {
            public string? StartTime { get; set; }
            public string? EndTime { get; set; }
            public string? TimeLine { get; set; }
            public Point[]? Points { get; set; }
            
            public class Point {
                public long PointId { get; set; }
                public float Value { get; set; }

                public Point(long pointId, float value) {
                    PointId = pointId;
                    Value = value;
                }
            }
        }
        public enum ETimeValue {
            FiveSek,
            OneMin,
            OneH,
            OneDay,
        }
        public class TsTimeLineAll {
            public TsTimeLine? Temp { get; set; }
            public TsTimeLine? WindSpeed { get; set; }
            public TsTimeLine? Humidity { get; set; }
            public TsTimeLine? WindDirection { get; set; }

        }

        [HttpGet("all")]
        public TsTimeLineAll GetAll(string? startTime, string? endTime, string? timeValue) {
            if (!string.IsNullOrEmpty(startTime)) return new TsTimeLineAll();
            if (!string.IsNullOrEmpty(endTime)) return new TsTimeLineAll();
            if (!string.IsNullOrEmpty(timeValue)) return new TsTimeLineAll();
            throw new NotImplementedException("all");
        }
        
        [HttpGet("temp")]
        public TsTimeLine GetTemp(string? startTime, string? endTime, string? timeValue) {
            if (!string.IsNullOrEmpty(startTime)) return new TsTimeLine();
            if (!string.IsNullOrEmpty(endTime)) return new TsTimeLine();
            if (!string.IsNullOrEmpty(timeValue)) return new TsTimeLine();
            throw new NotImplementedException("GetTemp");
        }

        [HttpGet("windspeed")]
        public TsTimeLine GetWindSpeed(string? startTime, string? endTime, string? timeValue) {
            if (!string.IsNullOrEmpty(startTime)) return new TsTimeLine();
            if (!string.IsNullOrEmpty(endTime)) return new TsTimeLine();
            if (!string.IsNullOrEmpty(timeValue)) return new TsTimeLine();
            throw new NotImplementedException("GetWindSpeed");
        }

        [HttpGet("humidity")]
        public TsTimeLine GetHumidity(string? startTime, string? endTime, string? timeValue) {
            if (!string.IsNullOrEmpty(startTime)) return new TsTimeLine();
            if (!string.IsNullOrEmpty(endTime)) return new TsTimeLine();
            if (!string.IsNullOrEmpty(timeValue)) return new TsTimeLine();
            throw new NotImplementedException("Humidity");
        }

        [HttpGet("windDirection")]
        public TsTimeLine GetWindDirection(string? startTime, string? endTime, string? timeValue) {
            if (!string.IsNullOrEmpty(startTime)) return new TsTimeLine();
            if (!string.IsNullOrEmpty(endTime)) return new TsTimeLine();
            if (!string.IsNullOrEmpty(timeValue)) return new TsTimeLine();
            throw new NotImplementedException("WindDirection");
        }
    }
}