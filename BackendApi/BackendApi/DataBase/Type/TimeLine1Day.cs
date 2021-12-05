namespace BackendApi.DataBase.Type;

public class TimeLine1Day : TimeLineDb {
    public static explicit operator TimeLine1Day(Point point) {
        return new TimeLine1Day {
            Humidity = point.Humidity,
            Temp = point.Temp,
            CreateTime = point.TimeUtc.Ticks,
            WindDirection = point.WindDirection,
            WindSpeed = point.WindSpeed,
            DeadValue = false
        };
    }
}