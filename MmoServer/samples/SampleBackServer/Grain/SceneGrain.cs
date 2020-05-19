using System.Threading.Tasks;
using System.Collections.Generic;
using Orleans;
using FootStone.Core;

namespace Sample.Grain
{
    public class HeroInfo
    {
        public string idcode;
        public string username;
    }

    public class SceneGrain : Orleans.Grain, ISceneGr
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Dictionary<string, HeroInfo> heros = new Dictionary<string, HeroInfo>();

        public SceneGrain()
        {
        }

        public async Task<bool> JoinAsync(string idc)
        {
            bool ret = false;
            var acctGrain = this.GrainFactory.GetGrain<IAccountGr>(System.Guid.Empty);
            var info = await acctGrain.GetUserInfoAsync(idc);
            if (!(info is null))
            {
                var hero = new HeroInfo()
                {
                    idcode = idc,
                    username = info.username
                };
                heros.Add(idc, hero);
                ret = true;
            }

            return ret;
        }
    }
}
