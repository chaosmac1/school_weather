namespace BackendApi.Ulitis {
    public struct Point {
        internal bool Dead;
        internal long TimeGrap;
        public long TimeReal;
        public float Value;

        internal Point(long timeGrap, bool dead) : this() {
            Dead = dead;
        }

        public Point(long timeReal, float value) : this() {
            TimeReal = timeReal;
            Value = value;
            Dead = false;
        }

        internal Point(long timeGrap, long timeReal, float value) {
            TimeGrap = timeGrap;
            TimeReal = timeReal;
            Value = value;
            Dead = false;
        }
    }
}