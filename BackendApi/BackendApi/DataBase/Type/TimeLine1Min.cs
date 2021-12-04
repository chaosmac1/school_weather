namespace BackendApi.DataBase.Type;

public class TimeLine1Min : TimeLineDb {
    public static explicit operator TimeLine1Min(Point point) {
        return new TimeLine1Min {
            Humidity = point.Humidity,
            Temp = point.Temp,
            CreateTime = new DateTime(point.TimeUtc.Ticks, DateTimeKind.Utc),
            WindDirection = point.WindDirection,
            WindSpeed = point.WindSpeed,
            DeadValue = false
        };
    }
}