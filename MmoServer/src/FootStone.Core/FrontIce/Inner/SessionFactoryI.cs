// **********************************************************************
//
// Copyright (c) 2003-2018 ZeroC, Inc. All rights reserved.
//
// **********************************************************************
using Ice;
using System;
using System.Threading;
using System.Collections.Generic;

namespace FootStone.FrontIce
{
    class SessionFactoryI : ISessionFactoryDisp_
    {
        private Ice.Logger logger;
        private string serverName;

        public SessionFactoryI(string name, Ice.Logger log)
        {
            this.serverName = name;
            this.logger = log;
        }

        public override void CreateSession(ISessionPushPrx proxy, Current current = null)
        {
            if (!(current.con.getInfo() is Ice.TCPConnectionInfo connection))
            {
                logger.error($"A ## Type of current.con is not TCPConnectionInfo!!!");
                return;
            }

            var key = FootStone.Core.HashUnit.GetMd5Str(connection.remoteAddress + ":" + connection.remotePort);
            var sessionI = new SessionI(proxy, key);
            // Never close this connection from the client and turn on heartbeats with a timeout of 30s
            current.con.getInfo().connectionId = sessionI.Id;
            current.con.setACM(30, ACMClose.CloseOff, ACMHeartbeat.HeartbeatAlways);
            current.con.setCloseCallback(_ => DestroySession(key));

            IceFrontSessionExtensions.sessions.TryAdd(key, sessionI);
            logger.print($"Create session :{sessionI.Id},{serverName} sessions count:{IceFrontSessionExtensions.sessions.Count}");
        }

        public override void Shutdown(Ice.Current current)
        {
            current.adapter.getCommunicator().shutdown();

            var logger = current.adapter.getCommunicator().getLogger();
            logger.print("Ice Shutting downed!");
        }

        private void DestroySession(string key)
        {
            var ret = IceFrontSessionExtensions.sessions.Remove(key, out SessionI sessionI);
            sessionI.Unbind();
            if (ret)
            {
                logger.print($"{sessionI.Id} is destroyed from thread " + $"{Thread.CurrentThread.ManagedThreadId}," +
                    $"current sessions count:{IceFrontSessionExtensions.sessions.Count}.");
            }
        }
    }

}