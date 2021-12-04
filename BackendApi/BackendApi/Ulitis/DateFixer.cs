using BackendApi.ExtensionMethod;

namespace BackendApi.Ulitis; 

public static class DateFixer {
    /// <param name="time"> Iso8601String = "yyyyMMddTHH:mm:ssZ" </param>
    /// <param name="timeOffset"> -14 ... +14 </param>
    public static TimeSpan ToDbTime(string time, int timeOffset) => ToDbTime(TimeStringToTimeSpan(time), timeOffset);
        
    /// <param name="time"> add timeoffset to time </param>
    /// <param name="timeOffset"> -14 ... +14 </param>
    public static TimeSpan ToDbTime(TimeSpan time, int timeOffset)
        => new DateTimeOffset(time.Ticks, new TimeSpan((TimeSpan.TicksPerHour * timeOffset) * -1)).ToTimeSpanPlusOffset();

    /// <param name="time"> remove timeoffset from time </param>
    /// <param name="timeOffset"> -14 ... +14 </param>
    public static TimeSpan ToBrowserTime(TimeSpan time, int timeOffset)
        => new DateTimeOffset(time.Ticks, new TimeSpan(TimeSpan.TicksPerHour * timeOffset)).ToTimeSpanPlusOffset();

    /// <param name="time"> Iso8601String = "yyyyMMddTHH:mm:ssZ" </param>
    private static TimeSpan TimeStringToTimeSpan(string time) {
        var year = int.Parse(time.Substring(0, 4));
        var month = int.Parse(time.Substring(4, 2));
        var day = int.Parse(time.Substring(6, 2));
        var hour = int.Parse(time.Substring(9, 2));
        var min = int.Parse(time.Substring(12, 2));
        var sek = int.Parse(time.Substring(15, 2));
            
        return new TimeSpan(new DateTime(year, month, day, hour, min, sek, DateTimeKind.Utc).Ticks);
    }
}