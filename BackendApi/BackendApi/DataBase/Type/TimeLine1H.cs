namespace BackendApi.DataBase.Type;

public class TimeLine1H : TimeLineDb {
    public static explicit operator TimeLine1H(Point point) {
        return new TimeLine1H {
            Humidity = point.Humidity,
            Temp = point.Temp,
            CreateTime = point.TimeUtc.Ticks,
            WindDirection = point.WindDirection,
            WindSpeed = point.WindSpeed,
            DeadValue = false
        };
    }
}