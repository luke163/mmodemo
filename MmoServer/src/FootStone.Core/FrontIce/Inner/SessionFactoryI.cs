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

            var session = new SessionI(proxy);
            // Never close this connection from the client and turn on heartbeats with a timeout of 30s
            current.con.getInfo().connectionId = session.Id;
            current.con.setACM(30, ACMClose.CloseOff, ACMHeartbeat.HeartbeatAlways);
            current.con.setCloseCallback(_ => DestroySessionCallback(session));

            IceFrontSessionExtensions.sessions.TryAdd(session.Id, session);
            logger.print($"Create session :{session.Id},{serverName}");
        }

        public override void Shutdown(Ice.Current current)
        {
            current.adapter.getCommunicator().shutdown();

            var logger = current.adapter.getCommunicator().getLogger();
            logger.print("Ice Shutting downed!");
        }

        private void DestroySessionCallback(SessionI session)
        {
            IceFrontSessionExtensions.sessions.Remove(session.Id);
            IceFrontSessionExtensions.sessionBinds.TryRemove(session.Identity, out _);
            logger.print($"{session.Id} is destroyed from thread " + $"{Thread.CurrentThread.ManagedThreadId}");
        }
    }

}