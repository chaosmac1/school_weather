using System.Threading;
using BackendApi.DataBase;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace BackendApi; 

public class Program {
    public static async Task Main(string[] args) {
        StaticConf.Init();
            
        var (syncManger, syncMangerTask) = SyncManger.FactoryStart(5000);
        var iotServerTask = new IOTServer.IotServer(new IOTServer.IotServer.IotServerProps {
            Ipv4 = "192.168.2.21",
            Port = 3380,
            AllowKeys = new[] {"BrotMot"},
            MaxClients = 2,
            PackSize = 3000,
            PushStream = syncManger.GetIotPointWaiter()
        }).Start();
        
        var dbServerTask = new Rope(StaticConf.DbUrl, StaticConf.DbPasswd, syncManger.GetDbPointerWaiter()).Start();
        
        CreateHostBuilder(args).Build().Run();
        
        // while (true) {
        //     await Task.Delay(1000);
        // }
        
        syncMangerTask.Wait();
        iotServerTask.Wait();
        dbServerTask.Wait();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}