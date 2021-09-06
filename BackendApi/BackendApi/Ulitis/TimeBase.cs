using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace BackendApi.Ulitis {
    public class TimeBase {
        private long[] Ticks;
        private TimeLine[] TimeLines;

        public Queue<Point> InsertValues = new Queue<Point>(1000);

        public TimeBase(long[] ticks) {
            Ticks = ticks;
            TimeLines = new TimeLine[ticks.Length];
            for (int i = 0; i < ticks.Length; i++) {
                TimeLines[i] = new TimeLine(Ticks[i], 0);
            }
        }
        
        private class TimeLine {
            public long Ticks { get; private set; }
            private readonly List<Point> _points;
            private readonly List<Point> _pointmeags = new List<Point>();
            private Thread _thread;
            public TimeLine(long ticks, long startTime) {
                Ticks = ticks;
                _points = new List<Point>(17280);
                _points.Add(new Point() { Dead = true, TimeGrap = startTime});

                _thread = new Thread(ThreadFunc);
                _thread.Start();
            }

            public List<Point> ConvertTo(long ticksStart, long ticksSize) {
                List<Point> res = new List<Point>(1000);
                List<Point> buffer = new List<Point>(10);

                long startBuffer = -1; 
                
                foreach (var point in _points) {
                    if (point.TimeGrap < ticksStart) continue;
                    if (startBuffer == -1) {
                        buffer.Clear();
                        startBuffer = point.TimeGrap;
                    }
                    if (startBuffer + ticksSize < point.TimeGrap) {
                        startBuffer = -1;
                        res.Add(new Point() {
                            TimeGrap = buffer[0].TimeGrap,
                            TimeReal = buffer[0].TimeReal,
                            Value = buffer.Average(x => x.Value) / buffer.Count
                        });
                        res.Clear();
                    }
                    startBuffer = point.TimeGrap;
                }
                
                if (buffer.Count != 0) {
                    startBuffer = -1;
                    res.Add(new Point() {
                        TimeGrap = buffer[0].TimeGrap,
                        TimeReal = buffer[0].TimeReal,
                        Value = buffer.Average(x => x.Value) / buffer.Count
                    });
                    res.Clear();
                }

                return res;
            }
            
            public void Add(Point point) => _pointmeags.Add(point);

            private void Kill() => _thread.Join(10);
            private void ThreadFunc() {
                var ticksMs = (int)(Ticks / TimeSpan.TicksPerMillisecond);
                var lastStart = DateTime.Now.Millisecond;
                Thread.Sleep((int)Ticks);
                while (true) {
                    SetNextPoint();
                    
                    var newStart = DateTime.Now.Millisecond;
                    Thread.Sleep(ticksMs - (newStart - lastStart));
                }
            }
            
            public void DropAllThatLowerThen(long time) {
                var endPosi = -1;
                for (int i = 0; i < _points.Count; i++) {
                    if (_points[i].TimeGrap < time) endPosi = i;
                }
            
                if (endPosi == -1) return;

                for (int i = 0; i < endPosi + 1; i++) 
                    _points.RemoveAt(0);
            }

            public List<Point> Split(int points) {
                if (_points.Count <= points) return new List<Point>(0);
                var res = new List<Point>(_points.Count - points + 2);
                for (int i = 0; i < _points.Count - points; i++) {
                    res.Add(_points[0]);
                    _points.RemoveAt(0);
                }
                return res;
            }
            
            private void SetNextPoint() {
                var pointmeags = _pointmeags.ToList();
                if (pointmeags.Count == 0) _points.Add(new Point() { Dead = true, Value = 0, TimeGrap = _points[^1].TimeGrap + Ticks});
                
                _points.Add(new Point() {
                    Dead = false,
                    TimeGrap = _points[^1].TimeGrap + Ticks,
                    Value = pointmeags.Average(x => x.Value) 
                });
                
                for (int i = 0; i < pointmeags.Count; i++) 
                    _pointmeags.RemoveAt(0);
            } 
        }
    }
}