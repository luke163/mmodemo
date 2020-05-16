using Ice;
using Orleans;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using FootStone.Core;

namespace FootStone.FrontIce
{
    class FrontServer
    {
        private Communicator communicator;
        private Dictionary<string, SessionI> sessions;

        public FrontServer()
        {
            sessions = new Dictionary<string, SessionI>();
        }

        public void Init(IceOptions options, IClusterClient orleansClient)
        {
            try
            {
                var initData = new InitializationData();
                //设置日志输出              
                if (options.Logger == null) options.Logger = new NLogice(NLog.LogManager.GetLogger("Ice"));
                initData.logger = options.Logger;
                initData.properties = Util.createProperties();
                initData.properties.load(options.ConfigFile);

                communicator = Util.initialize(initData);
                var adapter = communicator.createObjectAdapter("SessionFactoryAdapter");
                var properties = communicator.getProperties();
                var id = Util.stringToIdentity(properties.getProperty("Identity"));
                var serverName = properties.getProperty("Ice.ProgramName");
                adapter.add(new SessionFactoryI(serverName, sessions, initData.logger), id);
                //添加facet
                foreach (var facet in options.FacetTypes)
                {
                    object[] args = new object[] { orleansClient };
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
