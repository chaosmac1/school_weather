#nullable enable
using System;

namespace BackendApi {
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
            ValueFromFile = new ConfValue {
                DbUrl = "mongodb://127.0.0.1",
                DbPasswd = "",
                IotIpv4 = "127.0.0.1"
            };
            // TODO StaticConf.Init()
        }

        private struct ConfValue {
            public string? DbUrl;
            public string? DbPasswd;
            public string? IotIpv4;
        }
    }
}