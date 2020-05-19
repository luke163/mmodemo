using System.Threading.Tasks;
using System.Collections.Generic;
using Orleans;
using FootStone.Core;

namespace Sample.Grain
{
    public class AccountGrain : Orleans.Grain, IAccountGr
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Dictionary<string, UserInfo> users = new Dictionary<string, UserInfo>();

        public AccountGrain()
        {
        }

        public Task<ResultRLTo> RegistOrLoginAsync(string account, string pwd)
        {
            ResultRLTo result = new ResultRLTo() { ret = 255 };
            var key = HashUnit.GetMd5Str(account);
            if (users.ContainsKey(key))
            {
                var player = users[key];
                if (!player.password.Equals(pwd))
                {
                    result.ret = 1; //密码不对
                    return Task.FromResult(result);
                }
            }
            else
            {
                users.Add(key, new UserInfo() { idcode = key, username = account, password = pwd });
            }
            result.ret = 0;
            result.idcode = key;
            result.username = account;
            result.password = pwd;

            logger.Debug($"玩家 {account} 来了 {key}");
            return Task.FromResult(result);
        }

        public Task<UserInfo> GetUserInfoAsync(string idcode)
        {
            UserInfo info = null;

            if (users.ContainsKey(idcode))
            {
                info = users[idcode];
            }

            return Task.FromResult(info);
        }
    }
}
