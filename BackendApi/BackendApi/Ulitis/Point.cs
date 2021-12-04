using System;

namespace BackendApi.Ulitis {
    public readonly struct Point {
        public readonly TimeSpan TimeUtc { get; }
        public readonly float Temp { get; }
        public readonly float WindSpeed { get; }
        public readonly float Humidity  { get; }
        public readonly float WindDirection { get; }

        public Point(TimeSpan timeUtc, float temp, float windSpeed, float humidity, float windDirection) : this() {
            TimeUtc = timeUtc;
            Temp = temp;
            WindSpeed = windSpeed;
            Humidity = humidity;
            WindDirection = windDirection;
        }
    }
}