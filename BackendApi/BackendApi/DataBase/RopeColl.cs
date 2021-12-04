using BackendApi.DataBase.Type;
using MongoDB.Driver;

namespace BackendApi.DataBase; 

public struct RopeColl {
    private readonly string _url;
    private readonly IMongoCollection<TimeLine1Day> _collTimeLine1day;
    private readonly IMongoCollection<TimeLine1H> _collTimeLine1H;
    private readonly IMongoCollection<TimeLine1Min> _collTimeLine1Min;
    private readonly IMongoCollection<TimeLine5Sek> _collTimeLine5Sek;

    private readonly System.Type _typeTimeLine5sek;
    private readonly System.Type _typeTimeLine1min;
    private readonly System.Type _typeTimeLine1h;
    private readonly System.Type _typeTimeLine1day;
    public RopeColl(string url) {
        _url = url;
        _collTimeLine5Sek = GetCol.GetTimeLine5sek(url) ?? throw new NullReferenceException("Connect To MongoDb Is Null");
        _collTimeLine1Min = GetCol.GetTimeLine1min(url) ?? throw new NullReferenceException("Connect To MongoDb Is Null");
        _collTimeLine1H = GetCol.GetTimeLine1h(url) ?? throw new NullReferenceException("Connect To MongoDb Is Null");
        _collTimeLine1day = GetCol.GetTimeLine1day(url) ?? throw new NullReferenceException("Connect To MongoDb Is Null");
        _typeTimeLine5sek = typeof(TimeLine5Sek);
        _typeTimeLine1min = typeof(TimeLine1Min);
        _typeTimeLine1h = typeof(TimeLine1H);
        _typeTimeLine1day = typeof(TimeLine1Day);
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
                Sort = Builders<T>.Sort.Descending(x => x.CreateTime)
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

    internal bool FindDocsInRange<T>(TimeSpan timeSpanStart, TimeSpan timeSpanEnd, out T[] timeLineDbs) where T : TimeLineDb {
        try {
            timeLineDbs = GetColl<T>().FindSync(
                Builders<T>.Filter.Gte(x => x.CreateTime, new DateTime(timeSpanStart.Ticks, DateTimeKind.Utc)) &
                Builders<T>.Filter.Lt(x => x.CreateTime, new DateTime(timeSpanStart.Ticks, DateTimeKind.Utc))).ToList().ToArray();
            return true;
        }
        catch (Exception) {
            timeLineDbs = Array.Empty<T>();
            return false;
        }
    }

    internal bool CreateColl() {
        try {
            var client = new MongoClient(_url).GetDatabase(GetCol.DataBaseName.TimeLine);
            client.CreateCollection(GetCol.CollationName.TimeLine5Sek, new CreateCollectionOptions());
            client.RunCommand<string>($"db.getCollection(\"{GetCol.CollationName.TimeLine5Sek}\")" + ".createIndex({\"CreateTime\":1, \"_id\":1},{unique: true,sparse: true,expireAfterSeconds: 600})");
            client.RunCommand<string>($"db.getCollection(\"{GetCol.CollationName.TimeLine1Min}\")" + ".createIndex({\"CreateTime\":1, \"_id\":1},{unique: true,sparse: true,expireAfterSeconds: 600})");
            client.RunCommand<string>($"db.getCollection(\"{GetCol.CollationName.TimeLine1day}\")" + ".createIndex({\"CreateTime\":1, \"_id\":1},{unique: true,sparse: true,expireAfterSeconds: 86400})");
        }
#if DEBUG
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
#else
            catch (Exception e) { return false; }
#endif
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