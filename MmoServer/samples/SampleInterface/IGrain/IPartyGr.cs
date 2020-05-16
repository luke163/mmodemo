using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Interface
{
    public interface IPartyGr : IGrainWithIntegerKey
    {
        Task<byte> Join(string account, string pwd);
    }
}
