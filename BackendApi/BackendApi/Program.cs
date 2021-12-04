using BackendApi.DataBase;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BackendApi; 

public class Program {
    public static void Main(string[] args) {
        StaticConf.Init();
            
        var (syncManger, syncMangerTask) = SyncManger.FactoryStart(5000);
        var iotServerTask = new IOTServer.IotServer(new IOTServer.IotServer.IotServerProps {
            Ipv4 = "127.0.0.1",
            Port = 3380,
            AllowKeys = new[] {"BrotMot"},
            MaxClients = 2,
            PackSize = 3000,
            PushStream = syncManger.GetIotPointWaiter()
        }).Start();

        var dbServerTask = new Rope(StaticConf.DbUrl, StaticConf.DbPasswd, syncManger.GetDbPointerWaiter()).Start();

        CreateHostBuilder(args).Build().Run();
        syncMangerTask.Wait(10);
        iotServerTask.Wait(10);
        dbServerTask.Wait(10);
    }

    private static IHostBuilder CreateHostBuilder(string[] args) {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}