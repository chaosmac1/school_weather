namespace BackendApi.DataBase.Type;

public class TimeLine5Sek : TimeLineDb {
    public static explicit operator TimeLine5Sek(Point point) {
        return new TimeLine5Sek {
            Humidity = point.Humidity,
            Temp = point.Temp,
            CreateTime = point.TimeUtc.Ticks,
            WindDirection = point.WindDirection,
            WindSpeed = point.WindSpeed,
            DeadValue = false
        };
    }
}