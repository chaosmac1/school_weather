using BackendApi.DataBase.Type;
using BackendApi.ExtensionMethod;

namespace BackendApi.Controllers; 

public class ConTimeLineObj {
    public class TsChart {
        private Radio[]? _radios = null;

        public Radio[] Radios {
            get => _radios?? throw new NullReferenceException(nameof(_radios));
            set {
                if (_radios is null) throw new NullReferenceException(nameof(_radios));
                _radios = value;
            }
        }

        public static TsChart FactoryWindDirection(List<TimeLineDb> list, int timezoneOffset) {
            return new TsChart(
                windDirePoints: new TsTimeLine() {
                    Points =
                        (from i in list select new TsTimeLine.Point(new TimeSpan(i.CreateTime.Ticks), i.WindDirection, timezoneOffset))
                        .ToArray()
                },
                windSpeedPoints: new TsTimeLine() {
                    Points =
                        (from i in list select new TsTimeLine.Point(new TimeSpan(i.CreateTime.Ticks), i.WindSpeed, timezoneOffset)).ToArray()
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
                buffer[(int)(windDirePoints.Points![i].Value / radioGruppe)].Add(windSpeedPoints.Points[i].Value);
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

        public static TsTimeLine FactoryTemp(List<TimeLineDb> list, int timezoneOffset) {
            return new TsTimeLine {
                Points =
                    (from i in list select new TsTimeLine.Point(new TimeSpan(i.CreateTime.Ticks), i.Temp, timezoneOffset)).ToArray()
            };
        }

        public static TsTimeLine FactoryWindSpeed(List<TimeLineDb> list, int timezoneOffset) {
            return new TsTimeLine {
                Points =
                    (from i in list select new TsTimeLine.Point(new TimeSpan(i.CreateTime.Ticks), i.WindSpeed, timezoneOffset)).ToArray()
            };
        }

        public static TsTimeLine FactoryHumidity(List<TimeLineDb> list, int timezoneOffset) {
            return new TsTimeLine {Points = 
                (from i in list select new TsTimeLine.Point(new TimeSpan(i.CreateTime.Ticks), i.Humidity, timezoneOffset)).ToArray()};
        }
            
        public class Point {
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            public long PointId { get; set; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            public float Value { get; set; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            public string? Date { get; set; }
                
            public Point(TimeSpan date, float value, int timezoneOffset) {
                PointId = date.Ticks;
                Value = value;
                Date = new DateTimeOffset(date.Ticks, new TimeSpan(timezoneOffset)).ToDateTimeUtcPlusOffset().ToString();
            }
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