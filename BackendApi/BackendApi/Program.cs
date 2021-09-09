using BackendApi.DataBase;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BackendApi {
    public class Program {
        public static void Main(string[] args) {
            StaticConf.Init();
            var syncManger = SyncManger.FactoryStart(5000);
            var iotServerTask = new IOTServer.IotServer(new IOTServer.IotServer.IotServerProps {
                Ipv4 = StaticConf.IotIpv4,
                Port = 3370,
                AllowKeys = new[] {"BrotMot"},
                MaxClients = 2,
                PackSize = 3000,
                PushStream = syncManger.SyncManger.GetIotPointWaiter()
            }).Start();

            var dbServerTask = new Rope(StaticConf.DbUrl, StaticConf.DbPasswd, syncManger.SyncManger.GetDbPointerWaiter()).Start();

            CreateHostBuilder(args).Build().Run();
            syncManger.Task.Wait(10);
            iotServerTask.Wait(10);
            dbServerTask.Wait(10);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}