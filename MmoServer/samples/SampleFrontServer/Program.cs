using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Threading.Tasks;
using FootStone.Core;
using FootStone.FrontOrleans;

namespace FootStone.SampleFrontServer
{
    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static string mysqlConnectCluster = "server=192.168.56.104;Port=3306;user id=root;password=456789;database=footstone;MaximumPoolsize=50";

        static void Main(string[] args)
        {
            try
            {
                Startup(args).Wait();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static Task Startup(string[] args)
        {
            IHostBuilder hostBuilder = new HostBuilder();

            //使用Orleans client
            hostBuilder.UseOrleansClient(clientBuilder =>
            {
                clientBuilder.UseAdoNetClustering(options =>
                {
                    options.ConnectionString = mysqlConnectCluster;
                    options.Invariant = "MySql.Data.MySqlClient";
                });
                clientBuilder.Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "luke";
                    options.ServiceId = "Sample";
                });
            });

            //使用Ice
            hostBuilder.UseFrontIce();

            hostBuilder.ConfigureServices(services =>
            {
                services.Configure<ConsoleLifetimeOptions>(_ => _.SuppressStatusMessages = true);
            });
            hostBuilder.ConfigureLogging(builder => builder.AddProvider(new NLoggerProvider()));

            return hostBuilder.RunConsoleAsync();
        }
    }
}
