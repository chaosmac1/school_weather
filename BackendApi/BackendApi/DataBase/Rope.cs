using System;
using System.Threading.Tasks;
using BackendApi.DataBase.Type;
using BackendApi.Ulitis;
using MongoDB.Driver;

namespace BackendApi.DataBase {
    
    public class Rope {
        private readonly string Url;
        private readonly string Passwd;
        private readonly IWaitPop<Point?> PopWait;
        private readonly IMongoCollection<TimeLine5sek> CollTimeLine5sek;
        private readonly IMongoCollection<TimeLine1min> CollTimeLine1min;
        private readonly IMongoCollection<TimeLine1h> CollTimeLine1h;
        private readonly IMongoCollection<TimeLine1day> CollTimeLine1day;
        
        public Rope(string url, string passwd, IWaitPop<Point?> popWait) {
            Url = url;
            Passwd = passwd;
            PopWait = popWait;
            CollTimeLine5sek = GetCol.GetTimeLine5sek(url)?? throw new NullReferenceException("Connect To MongoDb Is Null");
            CollTimeLine1min = GetCol.GetTimeLine1min(url)?? throw new NullReferenceException("Connect To MongoDb Is Null");
            CollTimeLine1h = GetCol.GetTimeLine1h(url)?? throw new NullReferenceException("Connect To MongoDb Is Null");
            CollTimeLine1day = GetCol.GetTimeLine1day(url)?? throw new NullReferenceException("Connect To MongoDb Is Null");
        }

        public static (Rope Rope, Task task) FactoryStart(string url, string passwd, IWaitPop<Point?> popWait) {
            var rope = new Rope(url, passwd, popWait);
            return (rope, rope.Start());
        }

        public Task Start() => Task.Run(LoopFunc);

        private void LoopFunc() {
            while (true) {
                var nextPoint = this.PopWait.Pop();
                if (nextPoint is null) continue;

                if (!FirstValueExist(out var firstValueExist)) 
                    throw new NotImplementedException();

                if (firstValueExist) {
                    
                }
            }
        }

        private bool InsertFirstValue() {
            
        }
        
        private bool FirstValueExist(out bool exist) {
            var db = GetColl<TimeLine5sek>();

            if (db.CountDocuments(Builders<TimeLine5sek>.Filter.Exists(x => x._Id)) == 0) {
                exist = false;
            }
            exist = true;
            return true;
        }


        private IMongoCollection<T> GetColl<T>() where T : TimeLineDb {
            var want = typeof(T);
            if (want == typeof(TimeLine5sek)) return (IMongoCollection<T>) CollTimeLine5sek;  
            if (want == typeof(TimeLine1min)) return (IMongoCollection<T>) CollTimeLine1min; 
            if (want == typeof(TimeLine1h))  return (IMongoCollection<T>) CollTimeLine1h;
            if (want == typeof(TimeLine1day))  return (IMongoCollection<T>) CollTimeLine1day;
            throw new Exception("T type not found");
        }
        
        
        private bool GetLastFind<T>(IMongoCollection<T> db, out T? timeline) where T : Type.TimeLineDb {
            try {
                timeline = db.FindSync<T>(Builders<T>.Filter.Exists(x => x._Id), new FindOptions<T>() {
                    Sort = Builders<T>.Sort.Descending(x => x.TimeTicks)
                }).FirstOrDefault();
            }
            catch (Exception) {
                timeline = null;
                return ThrowErr("Try To Find Value From Db");
            }
            return true;
        }

        private bool GetLast<T>(out T? point) where T : TimeLineDb => GetLastFind(GetColl<T>(), out point);
        
        private static bool ThrowErr(string msg) {
#if DEBUG
            throw new Exception(msg);
#else
                return false;
#endif
        }

        public static bool Update1min() {
            
        }

        public static bool Update1h() {
            
        }

        public static bool Update1day() {
            
        }

        public static bool UpdateAll() {
            if (!Update1min()) return false;
            if (!Update1h()) return false;
            if (!Update1day()) return false;
            return true;
        }
    }
}






























