using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Grain
{
    public struct ResultRLTo
    {
        public byte ret;
        public string idcode;
        public string username;
        public string password;
    }

    public class UserInfo
    {
        public string idcode;
        public string username;
        public string password;
    }

    public interface IAccountGr : IGrainWithGuidKey
    {
        Task<ResultRLTo> RegistOrLoginAsync(string account, string pwd);

        Task<UserInfo> GetUserInfoAsync(string idcode);
    }
}
