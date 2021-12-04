#nullable enable
namespace BackendApi; 

public static class StaticConf {
    private static ConfValue? _valueFromFileBuffer;
    private static ConfValue ValueFromFile {
        get {
            if (_valueFromFileBuffer is null)
                throw new NullReferenceException("StaticConf.Init() Must be First Run");
            return _valueFromFileBuffer.Value;
        }
        set => _valueFromFileBuffer = value;
    }
    public static string DbUrl => ValueFromFile.DbUrl ?? throw new NullReferenceException("DbUrl");
    public static string DbPasswd => ValueFromFile.DbPasswd ?? throw new NullReferenceException("DbPasswd");
    public static string IotIpv4 => ValueFromFile.IotIpv4 ?? throw new NullReferenceException("IotIpv4");

    public static void Init() {
        ValueFromFile = new ConfValue("mongodb://127.0.0.1", "", "127.0.0.1");
        // TODO StaticConf.Init()
    }

    private readonly struct ConfValue {
        internal readonly string DbUrl;
        internal readonly string DbPasswd;
        internal readonly string IotIpv4;

        public ConfValue() 
            => throw new Exception($"{nameof(ConfValue)} default Construct not allow");
        
        internal ConfValue(string dbUrl, string dbPasswd, string iotIpv4) {
            DbUrl = dbUrl;
            DbPasswd = dbPasswd;
            IotIpv4 = iotIpv4;
        }
    }
}