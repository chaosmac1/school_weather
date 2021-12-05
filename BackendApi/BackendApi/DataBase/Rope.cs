using BackendApi.DataBase.Type;

namespace BackendApi.DataBase; 

public class Rope {
    private RopeColl _ropeColl;
    private readonly IWaitPop<Point?> _popWait;
    private bool _firstValuesExist;
    private readonly string _url;
    public Rope(string url, string passwd, IWaitPop<Point?> popWait) {
        _url = url;
        _popWait = popWait;
        _ropeColl = new RopeColl(url);
    }

    public static (Rope Rope, Task task) FactoryStart(string url, string passwd, IWaitPop<Point?> popWait) {
        var rope = new Rope(url, passwd, popWait);
        return (rope, rope.Start());
    }

    public Task Start() => Task.Run(() => {
        _ropeColl.CreateColl();
        LoopFunc();
    });

    private void LoopFunc() {
        while (true) {
            var nextPoint = _popWait.Pop();
            if (nextPoint is null) continue;
            var ss = new DateTime(nextPoint.Value.TimeUtc.Ticks, DateTimeKind.Utc);
            if (_firstValuesExist == false) {
                if (!_ropeColl.FirstValueExistInDb<TimeLine5Sek>(out _firstValuesExist)) {
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
            var timeLine = (TimeLine5Sek) point;
            _ropeColl.GetColl<TimeLine5Sek>().InsertOne(timeLine);
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
            _ropeColl.GetColl<TimeLine5Sek>().InsertOne((TimeLine5Sek)point);
            _ropeColl.GetColl<TimeLine1Min>().InsertOne((TimeLine1Min)point);
            _ropeColl.GetColl<TimeLine1H>().InsertOne((TimeLine1H)point);
            _ropeColl.GetColl<TimeLine1Day>().InsertOne((TimeLine1Day)point);
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
    /// <typeparam name="TLow"> Lower Time Line </typeparam>
    /// <typeparam name="UHigh"> Higher Time Line </typeparam>
    /// <returns> Error == true </returns>
    private bool Update<TLow, UHigh>(TimeSpan timeLineRange, out bool canInsert) where TLow : TimeLineDb, new() where UHigh : TimeLineDb, new() {

        if (!_ropeColl.GetLastDoc<TLow>(out var tLast)) {
            canInsert = false;
            return ThrowErr( $"{nameof(tLast)} is null");
        }

        if (tLast is null) {
            canInsert = false;
            return true;
        }

        if (!_ropeColl.GetLastDoc<UHigh>(out var uLast)) {
            canInsert = false;
            return ThrowErr($"{nameof(uLast)} is null");
        }

        if (uLast is null) {
            canInsert = false;
            return true;
        }

        var now = tLast.CreateTime;
        
        var diff = now - (uLast.CreateTime + timeLineRange.Ticks);
        
        Console.WriteLine($"lower: {uLast.CreateTime} now: {new DateTime(now, DateTimeKind.Unspecified)} Type: {typeof(UHigh)} low > high: {diff > 0}");
        if (diff < 0) {
            canInsert = false;
            return true;
        }

        var ttStart = new TimeSpan(tLast.CreateTime).Subtract(timeLineRange);
        var ttEnd = new TimeSpan(tLast.CreateTime);
        if (!_ropeColl.FindDocsInRange<TLow>(ttStart, ttEnd, out var timeLineTs)) {
            canInsert = false;
            return ThrowErr(nameof(_ropeColl.FindDocsInRange) + "Error");
        }

        var creteTime = new TimeSpan(uLast.CreateTime + timeLineRange.Ticks);
        var insertValue = TimeLineDb.Average<TLow, UHigh>(creteTime, timeLineTs);

        canInsert = true;
        return InsertNextValueInDb(insertValue);
    }

    private static TimeSpan _UpdateAll1min = new(TimeSpan.TicksPerMinute);
    private static TimeSpan _UpdateAll1h = new(TimeSpan.TicksPerHour);
    private static TimeSpan _UpdateAll1Day = new(TimeSpan.TicksPerDay); 
    private bool UpdateAll() {
        bool UpdateLoop<T, U>(TimeSpan rangeTimeSpan) where T : TimeLineDb, new() where U : TimeLineDb, new() {
            while (true) {
                if (!Update<T, U>(rangeTimeSpan, out var canInsert)) 
                    return ThrowErr($"Update<>({typeof(T)},{typeof(U)})");
                if (!canInsert) return true;
            }
        }
            
        bool Update1Min() => UpdateLoop<TimeLine5Sek, TimeLine1Min>(_UpdateAll1min);
        bool Update1H() => UpdateLoop<TimeLine1Min, TimeLine1H>(_UpdateAll1h);
        bool Update1Day() => UpdateLoop<TimeLine1H, TimeLine1Day>(_UpdateAll1Day);
            
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