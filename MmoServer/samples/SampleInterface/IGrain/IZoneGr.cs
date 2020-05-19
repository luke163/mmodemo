using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Interface
{
    public interface IZoneGr : IGrainWithIntegerKey
    {
        Task SubscribeAsync(IZoneOb observer);

        Task UnSubscribeAsync(IZoneOb observer);
		
		Task PushMessageForTest(string msg);
    }
}
