using Orleans;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Interface
{
    public interface IZoneOb : IGrainObserver
    {
        void OnNext(string token);
    }
}
