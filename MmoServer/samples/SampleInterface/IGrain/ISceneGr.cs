using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Grain
{
    public interface ISceneGr : IGrainWithIntegerKey
    {
        Task<bool> JoinAsync(string idcode);
    }
}
