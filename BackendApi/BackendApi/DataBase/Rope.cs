using System;
using System.Linq;
using System.Threading.Tasks;
using BackendApi.DataBase.Type;
using BackendApi.Ulitis;
using MongoDB.Driver;

namespace BackendApi.DataBase {

    public class Rope {
        private readonly IMongoCollection<TimeLine1day> CollTimeLine1day;
        private readonly IMongoCollection<TimeLine1h> CollTimeLine1h;
        private readonly IMongoCollection<TimeLine1min> CollTimeLine1min;
        private readonly IMongoCollection<TimeLine5sek> CollTimeLine5sek;
        private readonly IWaitPop<Point?> PopWait;
        private bool FirstValuesExist;

        public Rope(string url, string passwd, IWaitPop<Point?> popWait) {
            PopWait = popWait;
            CollTimeLine5sek = GetCol.GetTimeLine5sek(url) ?? throw new NullReferenceException("Connect To MongoDb Is Null");
            CollTimeLine1min = GetCol.GetTimeLine1min(url) ?? throw new NullReferenceException("Connect To MongoDb Is Null");
            CollTimeLine1h = GetCol.GetTimeLine1h(url) ?? throw new NullReferenceException("Connect To MongoDb Is Null");
            CollTimeLine1day = GetCol.GetTimeLine1day(url) ?? throw new NullReferenceException("Connect To MongoDb Is Null");
        }

        public static (Rope Rope, Task task) FactoryStart(string url, string passwd, IWaitPop<Point?> popWait) {
            var rope = new Rope(url, passwd, popWait);
            return (rope, rope.Start());
        }

        public Task Start() {
            return Task.Run(LoopFunc);
        }

        private void LoopFunc() {
            while (true) {
                var nextPoint = PopWait.Pop();
                if (nextPoint is null) continue;

                if (!FirstValueExist(out FirstValuesExist)) {
                    ThrowErr("Try To Check If First Value Exist");
                    continue;
                }

                if (FirstValuesExist == false) {
                    if (!InsertFirstValue(nextPoint.Value)) {
                        ThrowErr("InsertFirstValue Insert Error");
                        continue;
                    }
                    FirstValuesExist = true;
                }

                if (!InsertPointIn5Sek(nextPoint.Value)) {
                    ThrowErr("InsertPointIn5sek Insert Error");
                    continue;
                }

                if (!UpdateAll()) ThrowErr("UpdateAll Error");
            }
        }

        private bool InsertPointIn5Sek(Point point) {
            try {
                GetColl<TimeLine5sek>().InsertOne((TimeLine5sek)point);
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
                GetColl<TimeLine5sek>().InsertOne((TimeLine5sek)point);
                GetColl<TimeLine1min>().InsertOne((TimeLine1min)point);
                GetColl<TimeLine1h>().InsertOne((TimeLine1h)point);
                GetColl<TimeLine1day>().InsertOne((TimeLine1day)point);
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

        private bool FirstValueExist(out bool exist) {
            var db = GetColl<TimeLine5sek>();

            if (db.CountDocuments(Builders<TimeLine5sek>.Filter.Exists(x => x._Id)) == 0) exist = false;
            exist = true;
            return true;
        }


        private IMongoCollection<T> GetColl<T>() where T : TimeLineDb {
            var want = typeof(T);
            if (want == typeof(TimeLine5sek)) return (IMongoCollection<T>)CollTimeLine5sek;
            if (want == typeof(TimeLine1min)) return (IMongoCollection<T>)CollTimeLine1min;
            if (want == typeof(TimeLine1h)) return (IMongoCollection<T>)CollTimeLine1h;
            if (want == typeof(TimeLine1day)) return (IMongoCollection<T>)CollTimeLine1day;
            throw new Exception("T type not found");
        }


        private static bool GetLastFind<T>(IMongoCollection<T> db, out T? timeline) where T : TimeLineDb {
            try {
                timeline = db.FindSync(Builders<T>.Filter.Exists(x => x._Id), new FindOptions<T> {
                    Sort = Builders<T>.Sort.Descending(x => x.TimeTicks)
                }).FirstOrDefault();
            }
            catch (Exception) {
                timeline = null;
                return ThrowErr("Try To Find Value From Db");
            }
            return true;
        }

        private bool GetLast<T>(out T? point) where T : TimeLineDb {
            return GetLastFind(GetColl<T>(), out point);
        }

        private bool FindAllDocsInRange<T>(long ticksStart, long ticksEnd, out T[] timeLineDbs) where T : TimeLineDb {
            try {
                timeLineDbs = GetColl<T>().FindSync(
                    Builders<T>.Filter.Gte(x => x.TimeTicks, ticksStart) &
                    Builders<T>.Filter.Lt(x => x.TimeTicks, ticksEnd)).ToList().ToArray();
                return true;
            }
            catch (Exception) {
                timeLineDbs = Array.Empty<T>();
                return false;
            }
        }

        /// <summary> </summary>
        /// <param name="timeTicks"></param>
        /// <param name="lowerTimeLine"></param>
        /// <typeparam name="T"> Higher Time Line </typeparam>
        /// <typeparam name="U"> Lower Time Line </typeparam>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        private bool InsertNextValueInDb<T, U>(long timeTicks, U[] lowerTimeLine) where T : TimeLineDb, new() where U : TimeLineDb, new() {
            try {
                GetColl<T>().InsertOne(new T {
                    TimeTicks = timeTicks,
                    Humidity = lowerTimeLine.Length == 0 ? -1 : lowerTimeLine.Average(x => x.Humidity),
                    Temp = lowerTimeLine.Length == 0 ? -1 : lowerTimeLine.Average(x => x.Temp),
                    WindDirection = lowerTimeLine.Length == 0 ? -1 : lowerTimeLine.Average(x => x.WindDirection),
                    WindSpeed = lowerTimeLine.Length == 0 ? -1 : lowerTimeLine.Average(x => x.WindSpeed)
                });
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
        /// <typeparam name="T"> Higher Time Line </typeparam>
        /// <typeparam name="U"> Lower Time Line </typeparam>
        /// <returns></returns>
        private bool Update<T, U>(long timeLineRange) where T : TimeLineDb, new() where U : TimeLineDb, new() {
            if (!GetLast<T>(out var lastTimeLine1Min))
                return false;

            if (lastTimeLine1Min is null)
                return true;

            if (lastTimeLine1Min.TimeTicks + timeLineRange > DateTime.Now.Ticks)
                return true;

            if (!FindAllDocsInRange<U>(lastTimeLine1Min.TimeTicks, lastTimeLine1Min.TimeTicks + timeLineRange, out var timeLineUs))
                return false;

            return InsertNextValueInDb<T, U>(lastTimeLine1Min.TimeTicks + timeLineRange, timeLineUs);
        }

        private bool Update1Min() {
            return Update<TimeLine1min, TimeLine5sek>(TimeSpan.TicksPerMinute);
        }

        private bool Update1H() {
            return Update<TimeLine1h, TimeLine1min>(TimeSpan.TicksPerHour);
        }

        private bool Update1Day() {
            return Update<TimeLine1day, TimeLine1h>(TimeSpan.TicksPerDay);
        }

        private bool UpdateAll() {
            if (!Update1Min()) return ThrowErr("Update1min Error");
            if (!Update1H()) return ThrowErr("Update1h Error");
            if (!Update1Day()) return ThrowErr("Update1day Error");
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