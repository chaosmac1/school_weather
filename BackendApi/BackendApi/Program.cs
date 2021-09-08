using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackendApi
{
    public class Program
    {
        public static void Main(string[] args) {
            StaticConf.Init();
            var syncManger = SyncManger.FactoryStart(5000);
            var iotServerTask = new IOTServer.IOTServer(new IOTServer.IOTServer.IOTServerProps() {
                Url = StaticConf.DbUrl,
                Port = 3370,
                AllowKeys = new []{"BrotMot"},
                MaxClients = 2,
                PackSize = 3000,
                PushStream = syncManger.SyncManger.GetIOTPointWaiter()
            }).Start();

            var dbServerTask = new DataBase.Rope(StaticConf.DbUrl, StaticConf.DbPasswd, syncManger.SyncManger.GetDbPointerWaiter());
            
            CreateHostBuilder(args).Build().Run();
            syncManger.Task.Wait(10);
            iotServerTask.Wait(10);
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}