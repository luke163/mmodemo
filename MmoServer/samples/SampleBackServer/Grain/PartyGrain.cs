using Sample.Interface;
using System.Threading.Tasks;
using System.Collections.Generic;
using Orleans;
using FootStone.Core;

namespace Sample.Grains
{
    struct UserInfo
    {
        public string username;
        public string password;
    }

    public class PartyGrain : Orleans.Grain, IPartyGr
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Dictionary<string, UserInfo> users = new Dictionary<string, UserInfo>();

        public PartyGrain()
        {
        }

        public async Task<byte> Join(string account, string pwd)
        {
            var key = HashUnit.GetMd5Hash(account);
            if (users.ContainsKey(key))
            {
                var player = users[key];
                if (!player.password.Equals(pwd))
                {
                    return 1; //密码不对
                }
            }
            else
            {
                users.Add(key, new UserInfo() { username = account, password = pwd });
            }
            logger.Debug($"{this.GetPrimaryKeyLong()} 号聚会 加入玩家 {account} - {pwd}");

            return 0;
        }
    }
}
