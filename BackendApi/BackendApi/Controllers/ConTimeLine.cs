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
        
        [HttpGet("temp")]
        public TsTimeLine GetTemp(string? startTime, string? endTime, string? timeValue) {
            throw new NotImplementedException("GetTemp");
        }

        [HttpGet("windspeed")]
        public TsTimeLine GetWindSpeed(string? startTime, string? endTime, string? timeValue) {
            throw new NotImplementedException("GetWindSpeed");
        }

        [HttpGet("humidity")]
        public TsTimeLine GetHumidity(string? startTime, string? endTime, string? timeValue) {
            throw new NotImplementedException("Humidity");
        }

        [HttpGet("windDirection")]
        public TsTimeLine GetWindDirection(string? startTime, string? endTime, string? timeValue) {
            throw new NotImplementedException("WindDirection");
        }
    }
}