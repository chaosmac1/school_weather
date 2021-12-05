namespace BackendApi.DataBase.Type;

public class TimeLine1Min : TimeLineDb {
    public static explicit operator TimeLine1Min(Point point) {
        return new TimeLine1Min {
            Humidity = point.Humidity,
            Temp = point.Temp,
            CreateTime = point.TimeUtc.Ticks,
            WindDirection = point.WindDirection,
            WindSpeed = point.WindSpeed,
            DeadValue = false
        };
    }
}