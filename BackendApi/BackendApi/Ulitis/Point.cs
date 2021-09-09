namespace BackendApi.Ulitis {
    public struct Point {
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public long TimeReal;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public float Temp;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public float WindSpeed;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public float Humidity;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public float WindDirection;

        public Point(long timeReal, float temp, float windSpeed, float humidity, float windDirection) : this() {
            TimeReal = timeReal;
            Temp = temp;
            WindSpeed = windSpeed;
            Humidity = humidity;
            WindDirection = windDirection;
        }
    }
}