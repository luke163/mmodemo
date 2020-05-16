using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Interface
{
    public interface IAccountGr : IGrainWithStringKey
    {
        Task Login(string sessionId, string account, string pwd);
    }
}
