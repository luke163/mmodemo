using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FootStone;
using FootStone.Client;
using Sample.Protocol;

namespace SampleClient
{
    //internal class PlayerPushI : IPlayerPushDisp_, IServerPush
    //{    
    //    private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    //    private SessionPushI sessionPushI;
    //    private string account;

    //    public PlayerPushI()
    //    {

    //    }

    //    public string GetFacet()
    //    {
    //       return typeof(IPlayerPushPrx).Name;
    //    }

    //    public void setSessionPushI(SessionPushI sessionPushI)
    //    {
    //        this.sessionPushI = sessionPushI;
    //    }

    //    public override void hpChanged(int hp, Current current = null)
    //    {
    //       // logger.Info(account + "begin hp changed::" + NetworkNew.HpChangeCount);
    //        Interlocked.Add(ref NetworkNew.HpChangeCount,1);
    //        if ((NetworkNew.HpChangeCount / NetworkNew.playerCount) > NetworkNew.lastHpChange)
    //        {
    //            Interlocked.Add(ref NetworkNew.lastHpChange, 1);
    //            logger.Info(account + " hp changed::" + NetworkNew.HpChangeCount);
    //        }
    //    }

    //    public void setAccount(string account)
    //    {
    //        this.account = account;
    //    }
    //}



    public class NetworkNew
    {
        private int playerCount = 0;

        public async Task Test(string ip, int port, int count, ushort startIndex, bool needNetty)
        {
            NLogger.Info($"begin test,count:${count},startIndex:{startIndex},needNetty:{needNetty}");

            var client = new FSClient();
            client.IceOptions(iceOptions =>
            {
                iceOptions.EnableDispatcher = false;
                //iceOptions.PushObjects.Add(new PlayerPushI());
                //iceOptions.PushObjects.Add(new ZonePushI());
            });

            //启动主线程
            //Thread thread = new Thread(new ThreadStart(async () =>
            //{
            //    do
            //    {
            //        client.Update();
            //        await Task.Delay(33);
            //     //   Thread.Sleep(33);
            //    } while (true);
            //}));
            //thread.Start();

            client.Start();

            for (ushort i = startIndex; i < startIndex + count; ++i)
            {
                var sessionId = "session" + i;
                var session = await client.CreateSession(ip, port);
                _ = RunSession(session, i, 20, needNetty);
                playerCount++;
                await Task.Delay(10);
            }
            NLogger.Info("all session created:" + count);
        }

        private async Task RunSession(IFSSession session, ushort index, int count, bool needNetty)
        {
            var account = "account" + index;
            var password = "111111";
            var playerName = "player" + index;

            try
            {
                session.SetDestroyedHandler((sender, e) =>
                {
                    NLogger.Info($"session:{session.GetId()} destroyed!");
                });

                //注册账号           
                var accountPrx = session.UncheckedCast(IAccountPoPrxHelper.uncheckedCast, IAccountPoPrxHelper.ice_staticId());
                try
                {
                    var ret = await accountPrx.LoginRequestAsync(account, password);
                    NLogger.Debug("RegisterRequest ok:" + account + ", result=" + ret);
                }
                catch (Ice.Exception ex)
                {
                    NLogger.Debug("RegisterRequest fail:" + ex.Message);
                }

                NLogger.Info($"{account} playerPrx end!");
            }
            catch (System.Exception e)
            {
                NLogger.Error(account + ":" + e.ToString());
            }
            finally
            {
                //session.Destory();
            }
        }

        //private async Task RunZone(IFSSession session, PlayerInfo playerInfo, int index)
        //{
        //    var zonePrx = session.UncheckedCast(IZonePrxHelper.uncheckedCast);

        //    System.Timers.Timer moveTimer = null;

        //    //进入Zone
        //    var endPoint = await zonePrx.BindZoneAsync(playerInfo.zoneId, playerInfo.playerId);
        //    //  await zonePrx.PlayerEnterAsync();

        //    //发送move消息
        //    moveTimer = new System.Timers.Timer();
        //    moveTimer.AutoReset = true;
        //    moveTimer.Interval = 500;
        //    moveTimer.Enabled = true;
        //    moveTimer.Elapsed += (_1, _2) =>
        //    {
        //        byte[] data = new byte[14];
        //        zonePrx.SendData(data);
        //    };
        //    moveTimer.Start();        
        //}
    }
}
