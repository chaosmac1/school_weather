#nullable enable
using System;

namespace BackendApi {
    public static class StaticConf {
        private struct ConfValue {
            public string? DbUrl;
            public string? DbPasswd;
        }

        private static ConfValue? _valueFromFileBuffer;
        private static ConfValue ValueFromFile {
            get {
                if (_valueFromFileBuffer is null)
                    throw new NullReferenceException("StaticConf.Init() Must be First Run");
                return _valueFromFileBuffer.Value;
            }
            set => _valueFromFileBuffer = value;
        }
        public static string DbUrl { get => ValueFromFile.DbUrl?? throw new NullReferenceException("DbUrl"); }
        public static string DbPasswd { get => ValueFromFile.DbPasswd?? throw new NullReferenceException("DbPasswd"); }

        public static void Init() {
            throw new NotImplementedException("StaticConf.Init()");
        }
    }
}