using System;
using BackendApi.DataBase.Type;
using MongoDB.Driver;

namespace BackendApi.DataBase {
    public static class GetCol {
        private static class DataBaseName {
            public const string TimeLine = "time_line";
        }

        private static class CollationName {
            public const string TimeLine5sek = "time_line_5sek";
            public const string TimeLine1min = "time_line_1min";
            public const string TimeLine1day = "time_line_1day";
            public const string TimeLine1h = "time_line_1h";
            public const string Acc = "acc";

        }

        private static class CollationTypeOf {
            public static readonly System.Type TTimeLine5sek = typeof(Type.TimeLine5sek);
            public static readonly System.Type TTimeLine1min = typeof(Type.TimeLine1min);
            public static readonly System.Type TTimeLine1day = typeof(Type.TimeLine1day);
            public static readonly System.Type TTimeLine1h = typeof(Type.TimeLine1h);   
        }
        
        private static IMongoClient GetMongoClient(string url) => new MongoClient(url); 
        
        public static IMongoCollection<T>? GetColByType<T>(string url) where T : Type.TimeLineDb {
            System.Type type = typeof(T);
            if (type == CollationTypeOf.TTimeLine5sek) return (IMongoCollection<T>?)GetTimeLine5sek(url); 
            else if (type == CollationTypeOf.TTimeLine1min) return (IMongoCollection<T>?)GetTimeLine1min(url); 
            else if (type == CollationTypeOf.TTimeLine1h) return (IMongoCollection<T>?)GetTimeLine1h(url);
            else if (type == CollationTypeOf.TTimeLine1day) return (IMongoCollection<T>?)GetTimeLine1day(url);
            return null;
        }
        public static IMongoCollection<TimeLine5sek>? GetTimeLine5sek(string url) {
            try {
                var client = GetMongoClient(url);
                var coll = client.GetDatabase(DataBaseName.TimeLine).GetCollection<TimeLine5sek>(CollationName.TimeLine5sek);
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
        public static IMongoCollection<TimeLine1min>? GetTimeLine1min(string url) {
            try {
                var client = GetMongoClient(url);
                var coll = client.GetDatabase(DataBaseName.TimeLine).GetCollection<TimeLine1min>(CollationName.TimeLine1min);
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
        public static IMongoCollection<TimeLine1day>? GetTimeLine1day(string url) {
            try {
                var client = GetMongoClient(url);
                var coll = client.GetDatabase(DataBaseName.TimeLine).GetCollection<TimeLine1day>(CollationName.TimeLine1day);
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
        public static IMongoCollection<TimeLine1h>? GetTimeLine1h(string url) {
            try {
                var client = GetMongoClient(url);
                var coll = client.GetDatabase(DataBaseName.TimeLine).GetCollection<TimeLine1h>(CollationName.TimeLine1h);
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

        public static IMongoCollection<Type.Acc>? GetAcc(string url) {
            try {
                var client = GetMongoClient(url);
                var coll = client.GetDatabase(DataBaseName.TimeLine).GetCollection<Type.Acc>(CollationName.Acc);
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
    }
}