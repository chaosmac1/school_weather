using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using BackendApi.Ulitis;
using Newtonsoft.Json;

namespace BackendApi.IOTServer {
    public class IotServer {
        private readonly string[] _allowKeys;
        private readonly IPAddress _ipv4;
        private readonly int _maxClients;
        private readonly uint _packSize;
        private readonly int _port;
        private readonly IWaitPush<Point?> _pushStream;
        private Socket? _socket;

        public IotServer(IotServerProps props) {
            if (string.IsNullOrEmpty(props.Ipv4)) throw new Exception("props.Ipv4 is null Or Empty");
            if (props.Ipv4 == "Any") _ipv4 = IPAddress.Any;
            else if (!IPAddress.TryParse(props.Ipv4, out _ipv4!)) throw new Exception("Ipv4 is not Valid");

            _socket = null;
            _allowKeys = props.AllowKeys ?? Array.Empty<string>();
            _port = props.Port == 0 ? throw new Exception("props.Port is 0") : props.Port;
            _packSize = props.Port == 0 ? throw new Exception("props.PackSize is 0") : props.PackSize;
            _maxClients = props.MaxClients;
            _pushStream = props.PushStream;
        }

        public Task Start() {
            return Task.Run(Run);
        }

        public void Run() {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(_ipv4, _port));
            _socket.Listen(_maxClients);

            Console.WriteLine(ToString());
            while (true) {
                Socket client = _socket.Accept();
                Console.WriteLine("Accept Client");

                SempClient.FactoryStart(client, _packSize, ServerClientFunc);
            }
        }

        private void ServerClientFunc(SempClient client) {
            static bool ThrowError(string msg) {
#if DEBUG
                throw new Exception(msg);
#else
                return false;
#endif
            }
            const string ok = "{error: false}";
            const string error = "{error: true}";

            while (client.Active()) {
                if (!client.ReceiveString(out var json)) {
                    _pushStream.Push(null);
                    ThrowError("client.ReceiveString()");
                }
                
                var createTime = DateTime.Now.Ticks;
                var iotTimeData = ReceiveIotValue.Factory(json!);

                if (iotTimeData is null) {
#if DEBUG
                    throw new NullReferenceException("iotTimeData Is Null");
#else
                    client.SendString(error);
                    client.Close();
#endif
                }

                if (_allowKeys.All(x => iotTimeData!.Key != x)) {
#if DEBUG
                    throw new Exception("Key not Same");
#else
                    client.SendString(error);
                    client.Close();
#endif
                }

                var pushTask = _pushStream.PushAsync(new Point());

                if (!client.SendString(ok)) {
                    pushTask.Wait();
                    ThrowError("client.SendString()");
                }
                pushTask.Wait();
            }
        }

        public override string ToString() {
            return "Socket Info:\n" +
                   $"  Ipv4: {_ipv4}\n" +
                   $"  Port: {_port}\n" +
                   $"  MaxClients: {_maxClients}";
        }

        public struct IotServerProps {
            public string? Ipv4;
            public int Port;
            public string[]? AllowKeys;
            public int MaxClients;
            public uint PackSize;
            public IWaitPush<Point?> PushStream;
        }

        private class ReceiveIotValue {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string? Key { get; set; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public float Temp { get; set; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public float WindSpeed { get; set; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public float Humidity { get; set; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public float WindDirection { get; set; }

            public Point ToPoint(long createTime) {
                return new(createTime, Temp, WindSpeed, Humidity, WindDirection);
            }

            public static ReceiveIotValue? Factory(string jsonString) {
                try {
                    return JsonConvert.DeserializeObject<ReceiveIotValue>(jsonString);
                }
                catch (Exception) {
                    return null;
                }
            }
        }
    }
}