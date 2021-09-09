using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using BackendApi.Ulitis;
using Newtonsoft.Json;

namespace BackendApi.IOTServer {
    public class IOTServer {
        private Socket? Socket;
        private IPAddress Ipv4;
        private int MaxClients;
        private int Port;
        private string[] AllowKeys;
        private uint PackSize;
        private IWaitPush<Point?> PushStream;
        public struct IOTServerProps {
            public string? Url;
            public int Port;
            public string[]? AllowKeys;
            public int MaxClients;
            public uint PackSize;
            public Ulitis.IWaitPush<Point?> PushStream;
        }

        public IOTServer(IOTServerProps props) {
            if (string.IsNullOrEmpty(props.Url)) throw new Exception("props.Ipv4 is null Or Empty");
            if (props.Url == "Any") Ipv4 = IPAddress.Any;
            else if (!System.Net.IPAddress.TryParse(props.Url, out Ipv4!)) throw new Exception("Ipv4 is not Valid");
            
            Socket = null;
            AllowKeys = props.AllowKeys ?? Array.Empty<string>();
            Port = (int)(props.Port == 0? throw new Exception("props.Port is 0"): props.Port);
            PackSize = (props.Port == 0 ? throw new Exception("props.PackSize is 0") : props.PackSize);
            MaxClients = props.MaxClients;
            PushStream = props.PushStream;
        }

        public Task Start() => Task.Run(Run);
        
        public void Run() {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(new IPEndPoint(Ipv4, Port));
            Socket.Listen(MaxClients);
            
            Console.WriteLine(this.ToString());
            while (true) {
                Socket client;
                client = Socket.Accept();
                Console.WriteLine("Accept Client");
                    
                SempClient.FactoryStart(client, PackSize, this.ServerClientFunc);
            }
            
        }

        private class ReceivIOTValue {
            public string? Key { get; set; }
            public float Temp { get; set; }
            public float WindSpeed { get; set; }
            public float Humidity { get; set; }
            public float WindDirection { get; set; }

            public Point ToPoint(long createTime) => new Point(createTime, Temp, WindSpeed, Humidity, WindDirection);
            
            public static ReceivIOTValue? Factory(string jsonString) {
                try {
                    return JsonConvert.DeserializeObject<ReceivIOTValue>(jsonString);
                }
                catch (Exception) { return null; }
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
                    this.PushStream.Push(null);
                    ThrowError("client.ReceiveString()");
                }

                var createTime = DateTime.Now.Ticks;
                var iotTimeData = ReceivIOTValue.Factory(json);
                
                if (iotTimeData is null) {
#if DEBUG
                    throw new NullReferenceException("iotTimeData Is Null");
#else
                    client.SendString(error);
                    client.Close();
#endif
                }

                if (AllowKeys.All(x => iotTimeData!.Key != x)) {
#if DEBUG
                    throw new Exception("Key not Same");
#else
                    client.SendString(error);
                    client.Close();
#endif
                }
                
                var pushTask = this.PushStream.PushAsync(new Point());
                
                if (!client.SendString(ok)) {
                    pushTask.Wait();
                    ThrowError("client.SendString()");
                }
                pushTask.Wait();
            }
        }

        public override string ToString() {
            return $"Socket Info:\n" +
                   $"  Ipv4: {Ipv4}\n" +
                   $"  Port: {Port}\n" +
                   $"  MaxClients: {MaxClients}";
        }
    }
}



























