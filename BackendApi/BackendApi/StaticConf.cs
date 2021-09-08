using System;

namespace BackendApi {
    public static class StaticConf {
        private struct ConfValue {
            public string? DbUrl;
            public string? DbPasswd;
        }

        private static ConfValue? _valueFromFileBuffer;
        private static ConfValue _valueFromFile {
            get {
                if (_valueFromFileBuffer is null)
                    throw new NullReferenceException("StaticConf.Init() Must be First Run");
                return _valueFromFileBuffer.Value;
            }
            set => _valueFromFileBuffer = value;
        }
        public static string DbUrl { get => _valueFromFile.DbUrl?? throw new NullReferenceException("DbUrl"); }
        public static string DbPasswd { get => _valueFromFile.DbPasswd?? throw new NullReferenceException("DbPasswd"); }

        public static void Init() {
            throw new NotImplementedException("StaticConf.Init()");
        }
    }
}