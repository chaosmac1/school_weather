using System;
using BackendApi.DataBase.Type;
using MongoDB.Driver;

namespace BackendApi.DataBase {
    public struct RopeColl {
        private readonly IMongoCollection<TimeLine1day> _collTimeLine1day;
        private readonly IMongoCollection<TimeLine1h> _collTimeLine1H;
        private readonly IMongoCollection<TimeLine1min> _collTimeLine1Min;
        private readonly IMongoCollection<TimeLine5sek> _collTimeLine5Sek;

        private readonly System.Type _typeTimeLine5sek;
        private readonly System.Type _typeTimeLine1min;
        private readonly System.Type _typeTimeLine1h;
        private readonly System.Type _typeTimeLine1day;
        public RopeColl(string url) {
            _collTimeLine5Sek = GetCol.GetTimeLine5sek(url) ?? throw new NullReferenceException("Connect To MongoDb Is Null");
            _collTimeLine1Min = GetCol.GetTimeLine1min(url) ?? throw new NullReferenceException("Connect To MongoDb Is Null");
            _collTimeLine1H = GetCol.GetTimeLine1h(url) ?? throw new NullReferenceException("Connect To MongoDb Is Null");
            _collTimeLine1day = GetCol.GetTimeLine1day(url) ?? throw new NullReferenceException("Connect To MongoDb Is Null");
            _typeTimeLine5sek = typeof(TimeLine5sek);
            _typeTimeLine1min = typeof(TimeLine1min);
            _typeTimeLine1h = typeof(TimeLine1h);
            _typeTimeLine1day = typeof(TimeLine1day);
        }
        
        internal IMongoCollection<T> GetColl<T>() where T : TimeLineDb {
            System.Type want = typeof(T);
            if (want == _typeTimeLine5sek) return (IMongoCollection<T>)_collTimeLine5Sek;
            if (want == _typeTimeLine1min) return (IMongoCollection<T>)_collTimeLine1Min;
            if (want == _typeTimeLine1h) return (IMongoCollection<T>)_collTimeLine1H;
            if (want == _typeTimeLine1day) return (IMongoCollection<T>)_collTimeLine1day;
            throw new Exception("T type not found");
        }
        
        internal bool GetLastDoc<T>(out T? lastDoc) where T : TimeLineDb => GetLastDoc(out lastDoc, GetColl<T>());
        
        internal bool GetLastDoc<T>(out T? lastDoc, IMongoCollection<T> db) where T : TimeLineDb {
            try {
                lastDoc = db.FindSync(Builders<T>.Filter.Exists(x => x._id), new FindOptions<T> {
                    Sort = Builders<T>.Sort.Descending(x => x.DateTime)
                }).FirstOrDefault();
                return true;
            }
            catch (Exception) {
                lastDoc = null;
                return ThrowErr("Try To Find Value From Db");
            }
        }
        
        internal bool FirstValueExistInDb<T>(out bool exist) where T : TimeLineDb {
            var count = GetColl<T>().CountDocuments(Builders<T>.Filter.Exists(x => x._id));
            exist = count != 0;
            return true;
        }

        internal bool FindDocsInRange<T>(DateTime dateTimeStart, DateTime dateTimeEnd, out T[] timeLineDbs) where T : TimeLineDb {
            try {
                timeLineDbs = GetColl<T>().FindSync(
                    Builders<T>.Filter.Gte(x => x.DateTime, dateTimeStart) &
                    Builders<T>.Filter.Lt(x => x.DateTime, dateTimeEnd)).ToList().ToArray();
                return true;
            }
            catch (Exception) {
                timeLineDbs = Array.Empty<T>();
                return false;
            }
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