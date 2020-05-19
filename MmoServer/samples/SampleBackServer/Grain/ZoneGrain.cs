using System.Threading.Tasks;
using System.Collections.Generic;
using Orleans;
using FootStone.Core;
using Sample.Interface;

namespace Sample.Grain
{
    public class ZoneGrain : Orleans.Grain, IZoneGr
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private ObserverSubscriptionManager<IZoneOb> _subsManager;

        public ZoneGrain()
        {
            _subsManager = new ObserverSubscriptionManager<IZoneOb>();
        }

        public Task SubscribeAsync(IZoneOb observer)
        {
            if (!_subsManager.IsSubscribed(observer))
            {
                _subsManager.Subscribe(observer);
                logger.Debug($"����IZoneOb");
            } else
            {
                logger.Debug($"�Ѿ����Ĺ�IZoneOb");
            }
            logger.Debug($"������ {_subsManager.Count}");

            return Task.CompletedTask;
        }

        public Task UnSubscribeAsync(IZoneOb observer)
        {
            _subsManager.Unsubscribe(observer);
            logger.Debug($"ȡ������IZoneOb, {_subsManager.Count}");

            return Task.CompletedTask;
        }

        public Task PushMessageForTest(string msg)
        {
            _subsManager.Notify(o => o.OnNext(msg));

            return Task.CompletedTask;
        }
    }
}
