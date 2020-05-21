using Ice;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FootStone.FrontIce;

namespace FootStone.Client
{
    public class SessionIce
    {
        public ISessionFactoryPrx SessionPrx { get; set; }
        public SessionPushI SessionPush { get; set; }

        public SessionIce(ISessionFactoryPrx sessionPrx, SessionPushI sessionPush)
        {
            this.SessionPrx = sessionPrx;
            this.SessionPush = sessionPush;
        }

        public void Destroy()
        {
            var connection = SessionPrx.ice_getConnection();
            if (!(connection is null)) connection.close(ConnectionClose.Forcefully);
        }
    }

    public class SessionPushI : ISessionPushDisp_
    {
        public event EventHandler OnDestroyed;

        internal SessionPushI()
        {
        }

        public override void SessionDestroyed(Current current = null)
        {
            OnDestroyed?.Invoke(this, EventArgs.Empty);
        }
    }

    /// ICE 网络类
    class NetworkIce
    {
        private List<Action> actions = new List<Action>();
        private IceClientOptions iceClientOptions;
        protected ObjectAdapter Adapter { get; private set; }
        protected Communicator Communicator { get; private set; }

        public NetworkIce(IceClientOptions iceClientOptions)
        {
            this.iceClientOptions = iceClientOptions;
        }

        /// 开启ICE
        public void Start()
        {
            try
            {
                var initData = new InitializationData();

                //设置属性
                if (iceClientOptions.Properties != null)
                {
                    initData.properties = iceClientOptions.Properties;
                }
                else
                {
                    initData.properties = Util.createProperties();
                }
                //initData.properties.setProperty("Ice.ACM.Client.Heartbeat", "Always");
                //initData.properties.setProperty("Ice.RetryIntervals", "-1");
                initData.properties.setProperty("Ice.FactoryAssemblies", "client");
                initData.properties.setProperty("Ice.Trace.Network", "1");
                initData.properties.setProperty("Ice.Default.Timeout", "15000");
                initData.logger = new NLogice();

                //设置dispatcher，由主线程调用
                if (iceClientOptions.EnableDispatcher)
                {
                    initData.dispatcher = delegate (Action action, Connection connection)
                    {
                        lock (this)
                        {
                            actions.Add(action);
                        }
                    };
                }

                Communicator = Util.initialize(initData);
                Adapter = Communicator.createObjectAdapter("");
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    Communicator.waitForShutdown();
                    NLogger.Info("ice closed!");
                }));

                thread.Start();
                NLogger.Info("ice started!");
            }
            catch (System.Exception ex)
            {
                NLogger.Error(ex.Message);
            }
        }

        /// 停止ICE
        public void Stop()
        {
            Communicator.shutdown();
            NLogger.Info("ice stopped!");
        }

        /// 创建session     
        public async Task<SessionIce> CreateSession(string ip, int port)
        {
            //设置locator
            var locator = LocatorPrxHelper.uncheckedCast(Communicator.stringToProxy("FootStone/Locator:default -h " + ip + " -p " + port));
            Communicator.setDefaultLocator(locator);

            //获取session factory
            var tmpconid = Guid.NewGuid().ToString("N");
            var sessionFactoryPrx = ISessionFactoryPrxHelper
                .uncheckedCast(Communicator.stringToProxy("sessionFactory")
                .ice_locatorCacheTimeout(0)
                .ice_connectionId(tmpconid)
                .ice_timeout(15000));

            //建立网络连接,设置心跳为30秒
            Connection connection = await sessionFactoryPrx.ice_getConnectionAsync();
            connection.setACM(30, ACMClose.CloseOff, ACMHeartbeat.HeartbeatAlways);
            NLogger.Debug(connection.getInfo().connectionId + " session connection:Endpoint=" + connection.getEndpoint().ToString());

            //添加push Prx
            var sessionPushI = new SessionPushI();
            var proxy = ISessionPushPrxHelper.uncheckedCast(Adapter.addWithUUID(sessionPushI));
            foreach (var proto in iceClientOptions.PushObjects)
            {
                Adapter.addFacet(proto.push, proxy.ice_getIdentity(), proto.name);
            }

            //监听连接断开事件,并且绑定该连接到adapter
            connection.setCloseCallback(_ =>
            {
                NLogger.Warn($"{tmpconid} connecton closed!");
                sessionPushI.SessionDestroyed();
            });
            connection.setAdapter(Adapter);

            //创建session,并且注册push Prx到服务器
            await sessionFactoryPrx.CreateSessionAsync(proxy);
            return new SessionIce(sessionFactoryPrx, sessionPushI);
        }

        /// 心跳更新，由主线程调用
        public void Update()
        {
            Action[] array;
            lock (this)
            {
                array = actions.ToArray();
                actions.Clear();
            }
            foreach (Action each in array)
            {
                each();
            }
        }
    }
}
