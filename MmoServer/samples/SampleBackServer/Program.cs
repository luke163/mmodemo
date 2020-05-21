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

namespace Sample.ClusterServer
{
    public class Program
    {
        private static readonly string IP_START = "192.168.56";
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
                silo.UseLocalhostClustering();
                //silo.UseAdoNetClustering(options =>
                //{
                //    options.ConnectionString = mysqlConnectCluster;
                //    options.Invariant = "MySql.Data.MySqlClient";
                //});
                silo.Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "luke";
                    options.ServiceId = "Sample";
                });
                silo.ConfigureEndpoints(GetLocalIP(), 11111, 30000);
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

        private static IPAddress GetLocalIP()
        {

            string name = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(name);


            foreach (IPAddress ipa in ipadrlist)
            {
                logger.Info($"本机地址 {ipa.ToString()}");
                if (ipa.ToString().StartsWith(IP_START))
                {
                    return ipa;
                }

            }
            return IPAddress.Loopback;
        }

    }
}
