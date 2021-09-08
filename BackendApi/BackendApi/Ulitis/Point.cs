namespace BackendApi.Ulitis {
    public struct Point {
        internal bool Dead;
        public long TimeReal;
        public float Temp;
        public float WindSpeed;
        public float Humidity;
        public float WindDirection;

        public Point(bool dead = true) : this() {
            Dead = dead;
        }

        public Point(long timeReal, float temp, float windSpeed, float humidity, float windDirection) : this() {
            Dead = false;
            TimeReal = timeReal;
            Temp = temp;
            WindSpeed = windSpeed;
            Humidity = humidity;
            WindDirection = windDirection;
        }
    }
}