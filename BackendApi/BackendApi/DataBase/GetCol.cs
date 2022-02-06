using BackendApi.DataBase.Type;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BackendApi.DataBase; 

public static class GetCol {

    private static IMongoClient GetMongoClient(string url) {
        return new MongoClient(url);
    }

    public static IMongoCollection<T>? GetColByType<T>(string url) where T : TimeLineDb {
        System.Type type = typeof(T);
            
        if (type == CollationTypeOf.TTimeLine5Sek) return (IMongoCollection<T>?)GetTimeLine5sek(url);
        if (type == CollationTypeOf.TTimeLine1Min) return (IMongoCollection<T>?)GetTimeLine1min(url);
        if (type == CollationTypeOf.TTimeLine1H) return (IMongoCollection<T>?)GetTimeLine1h(url);
        if (type == CollationTypeOf.TTimeLine1Day) return (IMongoCollection<T>?)GetTimeLine1day(url);
        
        return null;
    }

    public static IMongoCollection<TimeLine5Sek>? GetTimeLine5sek(string url) {
        try {
            var client = GetMongoClient(url);
            var isMongoLive = client.GetDatabase(DataBaseName.TimeLine)
                .RunCommandAsync((Command<BsonDocument>) "{ping:1}").Wait(1000);
            if (!isMongoLive) {
#if DEBUG
                throw new Exception("MongoDb Server Not Available");
#else
                return null;
#endif
                
            }
            var coll = client.GetDatabase(DataBaseName.TimeLine).GetCollection<TimeLine5Sek>(CollationName.TimeLine5Sek);
            return coll;
        }
#if DEBUG
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
#else
            catch (Exception) { return null; }
#endif
    }

    public static IMongoCollection<TimeLine1Min>? GetTimeLine1min(string url) {
        try {
            var client = GetMongoClient(url);
            var coll = client.GetDatabase(DataBaseName.TimeLine).GetCollection<TimeLine1Min>(CollationName.TimeLine1Min);
            return coll;
        }
#if DEBUG
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
#else
            catch (Exception) { return null; }
#endif
    }

    public static IMongoCollection<Log>? GetLog(string url) {
        try {
            var client = GetMongoClient(url);
            var coll = client.GetDatabase(DataBaseName.TimeLine).GetCollection<Log>(CollationName.Log);
            return coll;
        }
#if DEBUG
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
#else
            catch (Exception) { return null; }
#endif
    }
    
    public static IMongoCollection<TimeLine1Day>? GetTimeLine1day(string url) {
        try {
            var client = GetMongoClient(url);
            var coll = client.GetDatabase(DataBaseName.TimeLine).GetCollection<TimeLine1Day>(CollationName.TimeLine1day);
            return coll;
        }
#if DEBUG
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
#else
            catch (Exception) { return null; }
#endif
    }

    public static IMongoCollection<TimeLine1H>? GetTimeLine1h(string url) {
        try {
            var client = GetMongoClient(url);
            var coll = client.GetDatabase(DataBaseName.TimeLine).GetCollection<TimeLine1H>(CollationName.TimeLine1H);
            return coll;
        }
#if DEBUG
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
#else
            catch (Exception) { return null; }
#endif
    }

    public static IMongoCollection<Acc>? GetAcc(string url) {
        try {
            var client = GetMongoClient(url);
            var coll = client.GetDatabase(DataBaseName.TimeLine).GetCollection<Acc>(CollationName.Acc);
            return coll;
        }
#if DEBUG
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
#else
            catch (Exception) { return null; }
#endif
    }

    public static class DataBaseName {
        public const string TimeLine = "time_line";
    }

    public static class CollationName {
        public const string TimeLine5Sek = "time_line_5sek";
        public const string TimeLine1Min = "time_line_1min";
        public const string TimeLine1day = "time_line_1day";
        public const string TimeLine1H = "time_line_1h";
        public const string Acc = "acc";
        public const string Log = "log";
    }

    private static class CollationTypeOf {
        public static readonly System.Type TTimeLine5Sek = typeof(TimeLine5Sek);
        public static readonly System.Type TTimeLine1Min = typeof(TimeLine1Min);
        public static readonly System.Type TTimeLine1Day = typeof(TimeLine1Day);
        public static readonly System.Type TTimeLine1H = typeof(TimeLine1H);
        public static readonly System.Type TLog = typeof(Log);
    }
}