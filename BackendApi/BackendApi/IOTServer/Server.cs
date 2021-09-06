using System;
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
        private TimeBase TimeBase;
        public ref struct IOTServerProps {
            public string? Ipv4;
            public uint Port;
            public string[]? AllowKeys;
            public int MaxClients;
            public uint PackSize;
            public TimeBase? TimeBase;
        }

        public IOTServer(ref IOTServerProps props) {
            if (string.IsNullOrEmpty(props.Ipv4)) throw new Exception("props.Ipv4 is null Or Empty");
            if (props.Ipv4 == "Any") Ipv4 = IPAddress.Any;
            else if (!System.Net.IPAddress.TryParse(props.Ipv4, out Ipv4!)) throw new Exception("Ipv4 is not Valid");
            
            Socket = null;
            AllowKeys = props.AllowKeys ?? Array.Empty<string>();
            Port = (int)(props.Port == 0? throw new Exception("props.Port is 0"): props.Port);
            PackSize = (props.Port == 0 ? throw new Exception("props.PackSize is 0") : props.PackSize);
            MaxClients = props.MaxClients;
            TimeBase = props.TimeBase;
        }

        public Task Start() => Task.Run(Run);
        
        public void Run() {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(new IPEndPoint(Ipv4, Port));
            Socket.Listen(MaxClients);
            
            Console.WriteLine(this.ToString());
            while (true) {
                Socket client;
                try {
                    client = Socket.Accept();
                    Console.WriteLine("Accept Client");

                    SempClient.FactoryStart(client, PackSize, new[] {this.TimeBase}, ServerClientFunc);

                }
                catch (Exception e) {
#if DEBUG
                    throw;
#else
                    continue;
#endif
                }
                
                
            }
            
        }

        private class ReceivIOTValue {
            public string Key { get; set; }
            public float Temp { get; set; }
            public float WindSpeed { get; set; }
            public float Humidity { get; set; }
            public float WindDirection { get; set; }
            
            public static ReceivIOTValue? Factory(string jsonString) {
                try {
                    return JsonConvert.DeserializeObject<ReceivIOTValue>(jsonString);
                }
                catch (Exception) { return null; }
            } 
        }
        
        private bool ServerClientFunc((SempClient client, object[] objects) arg) {
            static bool ThrowError(string msg) {
#if DEBUG
                throw new Exception(msg);
#else
                return false;
#endif
            }
            const string ok = "{error: false}";
            const string error = "{error: true}";
            SempClient client = arg.client;
            TimeBase timeBase = arg.objects.Length == 0 ? throw new Exception("Array Length = 0"): (TimeBase)(arg.objects[0]);

            if (!client.ReceiveString(out var json)) 
                return ThrowError("client.ReceiveString()");

            var iotTimeData = ReceivIOTValue.Factory(json);

            if (iotTimeData is null) {
                if (!client.SendString(error)) 
                    return ThrowError("client.SendString()");    
            }
            
            if (!client.SendString(ok)) 
                return ThrowError("client.SendString()");
        }

        /*  {  
         *     key: string,
         *     temp: float,
         *     windSpeed: float,
         *     humidity: float,
         *     windDirection: float
         *  }
         */

        public override string ToString() {
            return $"Socket Info:\n" +
                   $"  Ipv4: {Ipv4}\n" +
                   $"  Port: {Port}\n" +
                   $"  MaxClients: {MaxClients}";
        }
    }
}