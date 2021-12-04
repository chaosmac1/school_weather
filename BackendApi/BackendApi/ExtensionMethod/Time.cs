namespace BackendApi.ExtensionMethod; 

public static class Time {
    public static DateTime ToDateTimeUtcPlusOffset(this DateTimeOffset self)
        => new(self.Ticks + self.Offset.Ticks, DateTimeKind.Utc);
        
    public static DateTime ToDateTimeUnspecifiedPlusOffset(this DateTimeOffset self)
        => new(self.Ticks + self.Offset.Ticks, DateTimeKind.Unspecified);

    public static TimeSpan ToTimeSpanPlusOffset(this DateTimeOffset self)
        => new TimeSpan(self.Ticks + self.Offset.Ticks);
}