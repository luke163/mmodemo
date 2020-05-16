using Orleans;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Interface
{
    public interface IAccountOb : IGrainObserver
    {
        void AccountLogined(string sessionId);
    }
}
