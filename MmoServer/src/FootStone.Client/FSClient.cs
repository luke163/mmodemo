using System;
using System.Threading;
using System.Threading.Tasks;

namespace FootStone.Client
{
    public class FSClient
    {
        private NetworkIce networkIce;
        private IceClientOptions iceOptions;
        private uint ccc = 0;

        public FSClient()
        {
            iceOptions = new IceClientOptions();
            networkIce = new NetworkIce(iceOptions);
        }

        public FSClient IceOptions(Action<IceClientOptions> configureIce)
        {
            configureIce(iceOptions);

            return this;
        }

        /// 创建session
        public async Task<FSSession> CreateSession(string ip, int port)
        {
            var sessionIce = await networkIce.CreateSession(ip, port);

            return new FSSession((++ccc).ToString(), sessionIce);
        }

        /// 启动客户端网络
        public void Start(CancellationToken cancellationToken = default(CancellationToken))
        {
            networkIce.Start();
        }

        /// 停止客户端网络
        public void Stop(CancellationToken cancellationToken = default(CancellationToken))
        {
            networkIce.Stop();
        }

        /// 定时刷新，注意必须要用主线程调用这个函数，以保证所有的网络回调都是在主线程运行
        public void Update()
        {
            networkIce.Update();
        }
    }
}