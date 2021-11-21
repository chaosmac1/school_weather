using System;
using System.Linq;
using System.Threading.Tasks;
using BackendApi.DataBase.Type;
using BackendApi.Ulitis;
using MongoDB.Driver;

namespace BackendApi.DataBase {

    public class Rope {
        private RopeColl _ropeColl;
        private readonly IWaitPop<Point?> _popWait;
        private bool _firstValuesExist;

        public Rope(string url, string passwd, IWaitPop<Point?> popWait) {
            _popWait = popWait;
            _ropeColl = new RopeColl(url);
        }

        public static (Rope Rope, Task task) FactoryStart(string url, string passwd, IWaitPop<Point?> popWait) {
            var rope = new Rope(url, passwd, popWait);
            return (rope, rope.Start());
        }

        public Task Start() => Task.Run(LoopFunc);

        private void LoopFunc() {
            while (true) {
                var nextPoint = _popWait.Pop();
                if (nextPoint is null) continue;

                if (_firstValuesExist == false) {
                    if (!_ropeColl.FirstValueExistInDb<TimeLine5sek>(out _firstValuesExist)) {
                        ThrowErr("Try To Check If First Value Exist");
                        continue;
                    }    
                }
                
                if (_firstValuesExist == false) {
                    if (!InsertFirstValue(nextPoint.Value)) {
                        ThrowErr("InsertFirstValue Insert Error");
                        continue;
                    }
                    _firstValuesExist = true;
                }
                
                else if (!InsertPointIn5Sek(nextPoint.Value)) {
                    ThrowErr("InsertPointIn5sek Insert Error");
                    continue;
                }

                if (!UpdateAll()) ThrowErr("UpdateAll Error");
            }
        }

        private bool InsertPointIn5Sek(Point point) {
            try {
                _ropeColl.GetColl<TimeLine5sek>().InsertOne((TimeLine5sek)point);
                return true;
            }
#if DEBUG
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
#else
            catch (Exception) { return false; }
#endif
        }

        private bool InsertFirstValue(Point point) {
            try {
                _ropeColl.GetColl<TimeLine5sek>().InsertOne((TimeLine5sek)point);
                _ropeColl.GetColl<TimeLine1min>().InsertOne((TimeLine1min)point);
                _ropeColl.GetColl<TimeLine1h>().InsertOne((TimeLine1h)point);
                _ropeColl.GetColl<TimeLine1day>().InsertOne((TimeLine1day)point);
                return true;
            }
#if DEBUG
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
#else
            catch (Exception) { return false; }
#endif
        }
        
        /// <summary>  </summary>
        /// <param name="timeLineRange"></param>
        /// <typeparam name="T"> Lower Time Line </typeparam>
        /// <typeparam name="U"> Higher Time Line </typeparam>
        /// <returns> Error == true </returns>
        private bool Update<T, U>(TimeSpan timeLineRange, out bool canInsert) where T : TimeLineDb, new() where U : TimeLineDb, new() {

            if (!_ropeColl.GetLastDoc<T>(out var tLast)) {
                canInsert = false;
                return ThrowErr( $"{nameof(tLast)} is null");
            }

            if (tLast is null) {
                canInsert = false;
                return true;
            }

            if (!_ropeColl.GetLastDoc<U>(out var uLast)) {
                canInsert = false;
                return ThrowErr($"{nameof(uLast)} is null");
            }

            if (uLast is null) {
                canInsert = false;
                return true;
            }

            var now = new TimeSpan(DateTime.Now.ToUniversalTime().Ticks);
            var uLastUT = new TimeSpan(uLast.DateTime.ToUniversalTime().Ticks);
            var diff = uLastUT.Add(timeLineRange).Subtract(now);
                
            Console.WriteLine($"lower: {uLast} now: {now} Type: {typeof(U)} lower > now: {diff.Ticks > 0}");
            if (diff.Ticks > 0) {
                canInsert = false;
                return true;
            }


            if (!_ropeColl.FindDocsInRange<T>(uLast.DateTime.AddTicks(1),
                uLast.DateTime.Add(timeLineRange), out var timeLineTs)) {
                canInsert = false;
                return ThrowErr(nameof(_ropeColl.FindDocsInRange) + "Error");
            }
                
            
            var insertValue = TimeLineDb.Average<T, U>(uLast.DateTime.Add(timeLineRange), timeLineTs);

            canInsert = true;
            return InsertNextValueInDb(insertValue);
        }

        private static TimeSpan _UpdateAll1min = new (new DateTime(0).AddMinutes(1).Ticks); 
        private static TimeSpan _UpdateAll1h = new (new DateTime(0).AddHours(1).Ticks); 
        private static TimeSpan _UpdateAll1Day = new (new DateTime(0).AddDays(1).Ticks); 
        private bool UpdateAll() {
            bool UpdateLoop<T, U>(TimeSpan rangeTimeSpan) where T : TimeLineDb, new() where U : TimeLineDb, new() {
                while (true) {
                    if (!Update<T, U>(rangeTimeSpan, out var canInsert)) 
                        return ThrowErr($"Update<>({typeof(T)},{typeof(U)})");
                    if (!canInsert) return true;
                }
            }
            
            bool Update1Min() => UpdateLoop<TimeLine5sek, TimeLine1min>(_UpdateAll1min);
            bool Update1H() => UpdateLoop<TimeLine1min, TimeLine1h>(_UpdateAll1h);
            bool Update1Day() => UpdateLoop<TimeLine1h, TimeLine1day>(_UpdateAll1Day);
            
            Console.WriteLine("------------------");
            if (!Update1Min()) return ThrowErr("Update1min Error");
            if (!Update1H()) return ThrowErr("Update1h Error");
            if (!Update1Day()) return ThrowErr("Update1day Error");
            Console.WriteLine("------------------");
            return true;
        }

        /// <summary> </summary>
        /// <param name="timeLine"> Value Insert In Db </param>
        /// <typeparam name="T"> Time Line </typeparam>
        /// <returns> Error == true </returns>
        private bool InsertNextValueInDb<T>(T timeLine) where T : TimeLineDb {
            try {
                _ropeColl.GetColl<T>().InsertOne(timeLine);
                return true;
            }
#if DEBUG
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
#else
            catch (Exception) { return false; }
#endif
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