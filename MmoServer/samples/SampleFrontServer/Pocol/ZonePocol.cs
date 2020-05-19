using Orleans;
using System.Threading.Tasks;
using System.Threading;
using FootStone.FrontIce;
using FootStone.FrontOrleans;
using Sample.Grain;
using Sample.Interface;

namespace Sample.Protocol
{
    internal class ZoneObserver : IZoneOb
    {

        public ZoneObserver()
        {
            System.Console.Out.WriteLine($"创建了监听者-对象哈希码 {this.GetHashCode()}，线程{Thread.CurrentThread.ManagedThreadId}");
        }

        public void OnNext(string token = "给我东西呀，不然看什么！！！")
        {
            System.Console.Out.WriteLine($"对象哈希码 {this.GetHashCode()}，{token}，线程{Thread.CurrentThread.ManagedThreadId}");
        }
    }

    public class ZonePocol : IZoneCoDisp_, IServantBase
    {
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public IClusterClient OrleansClient { get; private set; }
        public IObserverClient ObserverClient { get; private set; }

        public ZonePocol(IClusterClient Client1, IObserverClient Client2)
        {
            this.OrleansClient = Client1;
            this.ObserverClient = Client2;
        }

        public async override Task TestApiReqAsync(Ice.Current current)
        {
            var zoneGrain1 = OrleansClient.GetGrain<IZoneGr>(1);
            var zonekey1 = await ObserverClient.GetObserver<ZoneObserver, IZoneOb>(1);
            var ccc = ObserverClient.SubscribeWrap<IZoneOb>(zonekey1, ob => { zoneGrain1.SubscribeAsync(ob); });
            logger.Debug(zonekey1 + " -------- " + ccc);

            var zonekey2 = await ObserverClient.GetObserver<ZoneObserver, IZoneOb>(1);
            ccc = ObserverClient.SubscribeWrap<IZoneOb>(zonekey2, ob => { zoneGrain1.SubscribeAsync(ob); });
            logger.Debug(zonekey2 + " -------- " + ccc);


            var zoneGrain13 = OrleansClient.GetGrain<IZoneGr>(13);
            var zonekey3 = await ObserverClient.GetObserver<ZoneObserver, IZoneOb>(13);
            ccc = ObserverClient.SubscribeWrap<IZoneOb>(zonekey3, ob => { zoneGrain13.SubscribeAsync(ob); });
            logger.Debug(zonekey3 + " -------- " + ccc);
        }

        public async override Task TestApiReq2Async(Ice.Current current)
        {
            var zoneGrain1 = OrleansClient.GetGrain<IZoneGr>(1);

            await zoneGrain1.PushMessageForTest($"来自于grain一的消息{Thread.CurrentThread.ManagedThreadId}");
        }

        public async override Task TestApiReq3Async(Ice.Current current)
        {
            var zoneGrain13 = OrleansClient.GetGrain<IZoneGr>(13);

            await zoneGrain13.PushMessageForTest($"来自于grain十三的消息，啦啦啦啦啦啦啦啦{Thread.CurrentThread.ManagedThreadId}");
        }

    }
}
