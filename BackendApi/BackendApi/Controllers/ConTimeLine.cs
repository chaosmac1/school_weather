#if DEBUG
//#define TEST
#endif

using System;
using System.Collections.Generic;
using System.Globalization;
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
#if TEST
            TsTimeLine RandomValues(int min, int max, bool nextLimit = true) {
                long ticks = TimeSpan.TicksPerMinute;
                List<TsTimeLine.Point> points = new(10000);
                var random = new Random();
                var value = random.Next(min, max);
                
                for (long i = startTime; i < endTime; i += ticks) {
                    points.Add(new TsTimeLine.Point(i, value));
                    if (nextLimit == false) {
                        value = random.Next(min, max);
                    }
                    else if (nextLimit) {
                        value += random.Next(-3, 4);
                        value = value >= max ? max : value;
                        value = value <= min ? min : value;
                    }
                }

                var res = new TsTimeLine.Point[points.Count];
                
                
                return new TsTimeLine() {Points = points.ToArray()};
            } 

            var windSpeed = RandomValues(0, 100, true);
            
            return new TsTimeLineAll() {
                Humidity = RandomValues(0, 30),
                Temp = RandomValues(0, 30),
                WindDirection = new TsChart(
                    windSpeedPoints: windSpeed!, 
                    windDirePoints:RandomValues(0, 359, false)),
                WindSpeed = windSpeed
            };
#else
            if (!string.IsNullOrEmpty(timeValue)) return new TsTimeLineAll();
            
            if (!GetTimeLineFromServer(startTime, endTime, timeValue, out var timeLineDbs)) {
                ThrowErr("GetTimeLineFromServer Error");
                return new TsTimeLineAll();
            }

            return new TsTimeLineAll {
                Humidity = TsTimeLine.FactoryHumidity(timeLineDbs),
                Temp = TsTimeLine.FactoryTemp(timeLineDbs),
                WindDirection = TsChart.Factory(timeLineDbs),
                WindSpeed = TsTimeLine.FactoryWindSpeed(timeLineDbs)
            };
#endif
        }

        [HttpGet("temp")]
        public TsTimeLine GetTemp(long startTime, long endTime, string timeValue) {
            if (!string.IsNullOrEmpty(timeValue)) return new TsTimeLine();

            if (!GetTimeLineFromServer(startTime, endTime, timeValue, out var timeLineDbs)) {
                ThrowErr("GetTimeLineFromServer Error");
                return new TsTimeLine();
            }

            return new TsTimeLine {
                Points = (from i in timeLineDbs select new TsTimeLine.Point(i.DateTime.Ticks, i.Temp)).ToArray()
            };
        }
        //
        // [HttpGet("windspeed")]
        // public TsTimeLine GetWindSpeed(long startTime, long endTime, string timeValue) {
        //     if (!string.IsNullOrEmpty(timeValue)) return new TsTimeLine();
        //
        //     if (!GetTimeLineFromServer(startTime, endTime, timeValue, out var timeLineDbs)) {
        //         ThrowErr("GetTimeLineFromServer Error");
        //         return new TsTimeLine();
        //     }
        //
        //     return new TsTimeLine {
        //         Points = (from i in timeLineDbs select new TsTimeLine.Point(i.TimeTicks, i.WindSpeed)).ToArray()
        //     };
        // }
        //
        // [HttpGet("humidity")]
        // public TsTimeLine GetHumidity(long startTime, long endTime, string timeValue) {
        //     if (!string.IsNullOrEmpty(timeValue)) return new TsTimeLine();
        //
        //     if (!GetTimeLineFromServer(startTime, endTime, timeValue, out var timeLineDbs)) {
        //         ThrowErr("GetTimeLineFromServer Error");
        //         return new TsTimeLine();
        //     }
        //
        //     return new TsTimeLine {
        //         Points = (from i in timeLineDbs select new TsTimeLine.Point(i.TimeTicks, i.Humidity)).ToArray()
        //     };
        // }
        //
        //
        //
        // [HttpGet("windDirection")]
        // public TsTimeLine GetWindDirection(long startTime, long endTime, string timeValue) {
        //     if (!string.IsNullOrEmpty(timeValue)) return new TsTimeLine();
        //
        //     if (!GetTimeLineFromServer(startTime, endTime, timeValue, out var timeLineDbs)) {
        //         ThrowErr("GetTimeLineFromServer Error");
        //         return new TsTimeLine();
        //     }
        //
        //     return new TsTimeLine {
        //         Points = (from i in timeLineDbs select new TsTimeLine.Point(i.TimeTicks, i.WindDirection)).ToArray()
        //     };
        // }


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
                Builders<T>.Filter.Gte(x => x.DateTime, new DateTime(tickStart)) &
                Builders<T>.Filter.Lt(x => x.DateTime, new DateTime(tickEnd)),
                new FindOptions<T> {Sort = Builders<T>.Sort.Ascending(x => x.DateTime)}
            ).ToEnumerable().ToList();
            return true;
        }

        private static bool GetFromServer<T, U>(long tickStart, long tickEnd, out List<U>? res) where T : TimeLineDb where U : TimeLineDb {
            var db = GetCol.GetColByType<T>(StaticConf.DbUrl);
            res = new List<U>();
            if (db is null) return false;

            res = db.FindSync(
                Builders<T>.Filter.Gte(x => x.DateTime, new DateTime(tickStart)) &
                Builders<T>.Filter.Lt(x => x.DateTime, new DateTime(tickEnd)),
                new FindOptions<T> {Sort = Builders<T>.Sort.Ascending(x => x.DateTime)}
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
        
        public class TsChart {
            private Radio[]? _radios = null;

            public Radio[] Radios {
                get => _radios?? throw new NullReferenceException(nameof(_radios));
                set {
                    if (_radios is null) throw new NullReferenceException(nameof(_radios));
                    _radios = value;
                }
            }

            public static TsChart Factory(List<TimeLineDb> list) {
                return new TsChart(
                    windDirePoints: new TsTimeLine() {
                        Points =
                            (from i in list select new TsTimeLine.Point(i.DateTime.Ticks, i.WindDirection))
                            .ToArray()
                    },
                    windSpeedPoints: new TsTimeLine() {
                        Points =
                            (from i in list select new TsTimeLine.Point(i.DateTime.Ticks, i.WindSpeed)).ToArray()
                    }
                );
            }
            
            public TsChart(TsTimeLine windSpeedPoints, TsTimeLine windDirePoints) {
                static int Average(List<float> list) {
                    if (list.Count == 0) return 0;
                    long res = 0;
                    foreach (var i in list) res += (int)i;
                    if (res == 0) return 0;
                    return (int)(res / list.Count);
                }
                
                const int radioGruppe = 30;
                
                var buffer = new List<float>[360/30];
                for (int i = 0; i < buffer.Length; i++) buffer[i] = new List<float>();
                
                for (int i = 0; i < windSpeedPoints.Points!.Length; i++) {
                    buffer[(int)(windDirePoints.Points[i].Value / radioGruppe)].Add(windSpeedPoints.Points[i].Value);
                }

                
                
                this.Radios = new Radio[360/30];
                for (int i = 0; i < buffer.Length; i++) {
                    this.Radios[i] = new Radio((i * radioGruppe).ToString() + "°", Average(buffer[i]));
                }
            }

            public class Radio {
                public string Vector { get; set; }
                public int Value { get; set; }

                public Radio(string vector, int value) {
                    Vector = vector;
                    Value = value;
                }
            }
        }
        
        public class TsTimeLine {
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            public Point[]? Points { get; set; }

            public static TsTimeLine FactoryTemp(List<TimeLineDb> list) {
                return new TsTimeLine {
                    Points =
                        (from i in list select new TsTimeLine.Point(i.DateTime.Ticks, i.Temp)).ToArray()
                };
            }

            public static TsTimeLine FactoryWindSpeed(List<TimeLineDb> list) {
                return new TsTimeLine {
                    Points =
                        (from i in list select new TsTimeLine.Point(i.DateTime.Ticks, i.WindSpeed)).ToArray()
                };
            }

            public static TsTimeLine FactoryHumidity(List<TimeLineDb> list) {
                return new TsTimeLine {Points = 
                    (from i in list select new TsTimeLine.Point(i.DateTime.Ticks, i.Humidity)).ToArray()};
            }
            
            public class Point {

                public Point(long pointId, float value) {
                    PointId = pointId;
                    Value = value;
                    Date = new DateTime(pointId).ToString(CultureInfo.CurrentCulture);
                }
                
                // ReSharper disable once UnusedAutoPropertyAccessor.Global
                public long PointId { get; set; }
                // ReSharper disable once UnusedAutoPropertyAccessor.Global
                public float Value { get; set; }
                // ReSharper disable once UnusedAutoPropertyAccessor.Global
                public string? Date { get; set; }
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
            public TsChart? WindDirection { get; set; }
        }
    }
}