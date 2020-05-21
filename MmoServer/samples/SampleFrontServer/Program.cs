using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Threading.Tasks;
using FootStone.Core;
using FootStone.FrontIce;
using FootStone.FrontOrleans;
using Sample.Protocol;

namespace Sample.ClusterServer
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
                clientBuilder.UseLocalhostClustering();
                //clientBuilder.UseAdoNetClustering(options =>
                //{
                //    options.ConnectionString = mysqlConnectCluster;
                //    options.Invariant = "MySql.Data.MySqlClient";
                //});
                clientBuilder.Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "luke";
                    options.ServiceId = "Sample";
                });
            });

            //使用Ice
            hostBuilder.UseFrontService();

            hostBuilder.ConfigureServices(services =>
            {
                services.Configure<ConsoleLifetimeOptions>(_ => _.SuppressStatusMessages = true);
            });
            hostBuilder.ConfigureLogging(builder => builder.AddProvider(new NLoggerProvider()));

            return hostBuilder.RunConsoleAsync();
        }
    }

    /*************************************************************************************************/

    public static class SampleServiceHostingExtensions
    {
        private static void InitIceOptions(IceOptions iceOptions)
        {
            iceOptions.ConfigFile = "Ice.config";
            iceOptions.AddFacetType(typeof(PlayerPocol), IPlayerCoPrxHelper.ice_staticId());
            //iceOptions.FacetTypes.Add(typeof(WorldI));
            //iceOptions.FacetTypes.Add(typeof(PlayerI));
            //iceOptions.FacetTypes.Add(typeof(RoleMasterI));
            iceOptions.AddFacetType(typeof(ZonePocol), IZoneCoPrxHelper.ice_staticId());
        }

        public static IHostBuilder UseFrontService(this IHostBuilder builder)
        {
            builder.UseFrontObserver();
            builder.UseFrontIce(iceOptions => InitIceOptions(iceOptions));

            return builder;
        }
    }
}
