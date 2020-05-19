using Orleans;
using System.Threading.Tasks;
using FootStone.FrontIce;
using FootStone.FrontOrleans;
using Sample.Grain;

namespace Sample.Protocol
{
    public class PlayerPocol : IPlayerCoDisp_, IServantBase
    {
        //private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public IClusterClient OrleansClient { get; private set; }
		public IObserverClient ObserverClient { get; private set; }

        public PlayerPocol(IClusterClient Client1, IObserverClient Client2)
        {
            OrleansClient = Client1;
			ObserverClient = Client2;
        }

        public async override Task<RLResultRes> RegOrLoginReqAsync(string account, string pwd, Ice.Current current = null)
        {
            var acctGrain = OrleansClient.GetGrain<IAccountGr>(System.Guid.Empty); //统一的账户管理器
            var to = await acctGrain.RegistOrLoginAsync(account, pwd);
            var session = current.ObtainSessionByIPAddress();
            RLResultRes res = new RLResultRes() { ret = to.ret };
            if (!(session is null) && to.ret == 0)
            {
                res.idcode = to.idcode;
                session.Bind(to.idcode); //绑定应用层身份用于推送
                session.Push<string>("__identity__", to.idcode); //设置合法性标识用于验证是否拦截
            }

            return res;
        }

        public async override Task<bool> JoinSceneReqAsync(byte sceneid, Ice.Current current = null)
        {
            var session = current.ObtainSessionByIPAddress();
            if (!session.IsLegalIdentify())
            {
                return false;
            }

            var sceneGrain = OrleansClient.GetGrain<ISceneGr>(1); //仅一个场景
            var ret = await sceneGrain.JoinAsync(session.Identity);

            return ret;
        }
    }

    internal static class SampleFrontSessionExtensions
    {
        public static bool IsLegalIdentify(this SessionI session)
        {
            if (session is null) return false;
            var identify = session.Get<string>("__identity__");
            if (System.String.IsNullOrEmpty(identify)) return false;

            return true;
        }
    }
}
