using System;
using BackendApi.DataBase.Type;
using MongoDB.Driver;

namespace BackendApi.DataBase {
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

        public static IMongoCollection<TimeLine5sek>? GetTimeLine5sek(string url) {
            try {
                var client = GetMongoClient(url);
                var coll = client.GetDatabase(DataBaseName.TimeLine).GetCollection<TimeLine5sek>(CollationName.TimeLine5Sek);
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
                var coll = client.GetDatabase(DataBaseName.TimeLine).GetCollection<TimeLine1min>(CollationName.TimeLine1Min);
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
                var coll = client.GetDatabase(DataBaseName.TimeLine).GetCollection<TimeLine1h>(CollationName.TimeLine1H);
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

        private static class DataBaseName {
            public const string TimeLine = "time_line";
        }

        private static class CollationName {
            public const string TimeLine5Sek = "time_line_5sek";
            public const string TimeLine1Min = "time_line_1min";
            public const string TimeLine1day = "time_line_1day";
            public const string TimeLine1H = "time_line_1h";
            public const string Acc = "acc";
        }

        private static class CollationTypeOf {
            public static readonly System.Type TTimeLine5Sek = typeof(TimeLine5sek);
            public static readonly System.Type TTimeLine1Min = typeof(TimeLine1min);
            public static readonly System.Type TTimeLine1Day = typeof(TimeLine1day);
            public static readonly System.Type TTimeLine1H = typeof(TimeLine1h);
        }
    }
}