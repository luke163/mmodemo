using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orleans;
using System;
using System.Threading;
using System.Threading.Tasks;
using FootStone.FrontOrleans;

namespace FootStone.FrontIce
{
    public class IceFrontHostedService : IHostedService
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private FrontServer network = new FrontServer();

        public IceFrontHostedService(IOptions<IceOptions> options, IClusterClient client, IObserverClient client2)
        {
            logger.Info("IceFrontService Init!");
            network.Init(options.Value, client, client2);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            network.Start();
            logger.Info("IceFrontService Started!");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            network.Stop();
            logger.Info("IceFrontService Stopped!");
            return Task.CompletedTask;
        }
    }

    class FrontServer
    {
        private Ice.Communicator communicator;

        public FrontServer()
        {
        }

        public void Init(IceOptions options, IClusterClient orleansClient, IObserverClient observerClient)
        {
            try
            {
                var initData = new Ice.InitializationData();
                //设置日志输出              
                if (options.Logger == null) options.Logger = new FootStone.Core.NLogice(NLog.LogManager.GetLogger("Ice"));
                initData.logger = options.Logger;
                initData.properties = Ice.Util.createProperties();
                initData.properties.load(options.ConfigFile);

                communicator = Ice.Util.initialize(initData);
                var adapter = communicator.createObjectAdapter("SessionFactoryAdapter");
                var properties = communicator.getProperties();
                var id = Ice.Util.stringToIdentity(properties.getProperty("Identity"));
                var serverName = properties.getProperty("Ice.ProgramName");
                adapter.add(new SessionFactoryI(serverName, initData.logger), id);
                //添加facet
                foreach (var facet in options.FacetTypes)
                {
                    if (facet.type is IServantBase)
                    {
                        options.Logger.error($"Facet(${facet.type.Name}) must inherit IServantBase!!!");
                        continue;
                    }
                    object[] args = new object[] { orleansClient, observerClient };
                    var servant = (IServantBase)Activator.CreateInstance(facet.type, args);
                    var intercpetor = new FSInterceptor((Ice.Object)servant, options.Logger);
                    adapter.addFacet(intercpetor, id, facet.name);
                }
                adapter.activate();
                options.Logger.print("Ice inited!");
            }
            catch (Ice.Exception ex)
            {
                options.Logger.error("Ice init failed:" + ex.ToString());
            }
        }

        public void Start()
        {
            Task.Run(() => communicator.waitForShutdown());
            communicator.getLogger().print("Ice started!");
        }

        public void Stop()
        {
            communicator.getLogger().print("Ice shutdown!");
            communicator.shutdown();
        }
    }
}
