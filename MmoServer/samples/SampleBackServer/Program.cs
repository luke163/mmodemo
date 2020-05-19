using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;
using FootStone.Core;
using Sample.Grain;
using Sample.FrontServer;

namespace Sample.BackServer
{
    public class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static string mysqlConnectCluster = "Server=192.168.56.104;Port=3306;User Id=root;Password=456789;Database=footstone;MaximumPoolsize=50";

        public static void Main(string[] args)
        {
            try
            {
                Startup(args).Wait();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Console.ReadLine();
            }
        }

        private static Task Startup(string[] args)
        {
            IHostBuilder hostBuilder = new HostBuilder();
            //配置orlean的Silo
            hostBuilder.UseOrleans(silo =>
            {
                silo.UseAdoNetClustering(options =>
                {
                    options.ConnectionString = mysqlConnectCluster;
                    options.Invariant = "MySql.Data.MySqlClient";
                });
                silo.Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "luke";
                    options.ServiceId = "Sample";
                });
                silo.ConfigureEndpoints(IPAddress.Loopback, 11111, 30000);
                silo.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(AccountGrain).Assembly).WithReferences());
                //silo.AddAdoNetGrainStorage("ado1", options =>
                //{
                //    options.UseJsonFormat = true;
                //    options.ConnectionString = mysqlConnectStorage;
                //    options.Invariant = "MySql.Data.MySqlClient";
                //});
            });

            //添加ICE Front支持
            hostBuilder.UseFrontService();

            hostBuilder.ConfigureServices(services =>
            {
                services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);
            });
            hostBuilder.ConfigureLogging(builder => builder.AddProvider(new NLoggerProvider()));

            return hostBuilder.RunConsoleAsync();
        }
    }
}
