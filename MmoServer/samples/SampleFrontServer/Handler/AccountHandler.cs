using Ice;
using Orleans;
using System;
using System.Threading.Tasks;
using FootStone.FrontIce;
using Sample.Protocol;
using Sample.Interface;

namespace SampleFrontServer.Handler
{
    /// <summary>
    /// Observer class that implements the observer interface. Need to pass a grain reference to an instance of this class to subscribe for updates.
    /// </summary>
    //class AccountObserver : IAccountObserver
    //{
    //    private Session sessionI;

    //    private NLog.Logger logger = LogManager.GetCurrentClassLogger();

    //    public AccountObserver(Session sessionI)
    //    {
    //        this.sessionI = sessionI;
    //    }

    //    public void AccountLogined(string id)
    //    {
    //        logger.Debug("AccountLogined:" + id);
    //        if (!id.Equals(sessionI.Id))
    //        {
    //            logger.Debug("sessionI.CollocProxy.Destroy");
    //            sessionI.SessionPushPrx.ice_getConnection().close(ConnectionClose.Forcefully);
    //        }
    //    }
    //}


    public class AccountHandler : IAccountPoDisp_, IServantBase
    {
        //private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private IClusterClient orleansClient;

        public AccountHandler(IClusterClient orleansClient)
        {
            this.orleansClient = orleansClient;
        }

        public void Dispose()
        {
        }


        public async override Task<byte> LoginRequestAsync(string account, string pwd, Current current = null)
        {
            //获取accountGrain
            var grain = orleansClient.GetGrain<IPartyGr>(0);
            var ret = await grain.Join(account, pwd);

            return ret;
        }
    }
}
