using System;

namespace BackendApi.Ulitis {
    public struct Point
    {
        private long? _TimeReal;
        public long TimeReal { 
            get { return _TimeReal?? throw new NullReferenceException(nameof(_TimeReal));}
            set { _TimeReal = value;  } 
            }
        private float? _Temp;
        public float Temp { 
            get { return _Temp?? throw new NullReferenceException(nameof(_Temp));}
            set { _Temp = value;  } 
            }
        private float? _WindSpeed;
        public float WindSpeed { 
            get { return _WindSpeed?? throw new NullReferenceException(nameof(_WindSpeed));}
            set { _WindSpeed = value;  } 
            }
        private float? _Humidity;
        public float Humidity { 
            get { return _Humidity?? throw new NullReferenceException(nameof(_Humidity));}
            set { _Humidity = value;  } 
            }
        private float? _WindDirection;
        public float WindDirection { 
            get { return _WindDirection?? throw new NullReferenceException(nameof(_WindDirection));}
            set { _WindDirection = value;  } 
            }

        public Point(long timeReal, float temp, float windSpeed, float humidity, float windDirection) : this() {
            TimeReal = timeReal;
            Temp = temp;
            WindSpeed = windSpeed;
            Humidity = humidity;
            WindDirection = windDirection;
        }
    }
}