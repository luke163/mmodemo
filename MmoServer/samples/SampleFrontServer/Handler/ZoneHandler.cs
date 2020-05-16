//using FootStone.Core.GrainInterfaces;
//using FootStone.FrontIce;
//using FootStone.GrainInterfaces;
//using Ice;
//using Orleans;
//using System;
//using System.Threading.Tasks;
//using System.Timers;

//namespace SampleFrontIce
//{
//    //internal class StreamObserver : IAsyncObserver<byte[]>
//    //{
//    //    private string  name;
//    //    private IZonePushPrx push;
//    //    private int count = 0;


//    //    public StreamObserver(IZonePushPrx push, string name)
//    //    {
//    //        this.push = push;
//    //        this.name = name;
//    //    }

//    //    public Task OnCompletedAsync()
//    //    {
//    //        Console.Out.WriteLine(name + " receive completed");
//    //        return Task.CompletedTask;
//    //    }

//    //    public Task OnErrorAsync(System.Exception ex)
//    //    {
//    //        Console.Out.WriteLine(name + " receive error:" + ex.Message);
//    //        return Task.CompletedTask;
//    //    }

//    //    public Task OnNextAsync(byte[] item, StreamSequenceToken token = null)
//    //    {


//    //        //if (Global.ZoneMsgCount % 330000 == 0)
//    //        //{
//    //        //   Console.Out.WriteLine("zone msg count:" + Global.ZoneMsgCount);
//    //        //}
//    //        //  count++;
//    //     //   Global.ZoneMsgCount++;


//    //        //Console.Out.WriteLine(" receive bytes:" + item.Length);
//    //        //   push.begin_ZoneSync(item);
//    //        return Task.CompletedTask;
//    //    }
//    //}

//    public class ZoneHandler : IZoneDisp_, IServantBase
//    {
//        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


//        public ZoneHandler(Session session, IClusterClient orleansClient)
//        {
//            this.session = session;
//            this.orleansClient = orleansClient;
//        }

//        public string GetFacet()
//        {
//            return nameof(IZonePrx);
//        }
      
//        public void Dispose()
//        {
//            if(moveTimer != null)
//            {
//                moveTimer.Close();
//            }

//            if (zoneGrain != null)
//            {
//                zoneGrain.PlayerLeave(this.session.PlayerId);
//            }
//        }
      
//    }
//}
