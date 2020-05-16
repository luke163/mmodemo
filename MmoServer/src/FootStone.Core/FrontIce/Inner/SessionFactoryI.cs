// **********************************************************************
//
// Copyright (c) 2003-2018 ZeroC, Inc. All rights reserved.
//
// **********************************************************************
using Ice;
using System;
using System.Threading;
using System.Collections.Generic;
using FootStone.Core;

namespace FootStone.FrontIce
{
    class SessionFactoryI : ISessionFactoryDisp_
    {
        private Ice.Logger logger;
        private string serverName;
        private IDictionary<string, SessionI> sessions;

        public SessionFactoryI(string name, IDictionary<string, SessionI> dict, Ice.Logger log)
        {
            this.serverName = name;
            this.sessions = dict;
            this.logger = log;
        }

        public override void CreateSession(ISessionPushPrx proxy, Current current = null)
        {
            if (!(current.con.getInfo() is Ice.TCPConnectionInfo connection))
            {
                logger.error($"Type of current.con is not ConnectionInfo!!!");
                return;
            }

            var sessionI = new SessionI(proxy);
            var md5key = HashUnit.GetMd5Hash(connection.remoteAddress + ":" + connection.remotePort);
            // Never close this connection from the client and turn on heartbeats with a timeout of 30s
            current.con.getInfo().connectionId = sessionI.Id;
            current.con.setACM(30, ACMClose.CloseOff, ACMHeartbeat.HeartbeatAlways);
            current.con.setCloseCallback(_ => DestroySession(md5key));

            sessions.Add(md5key, sessionI);
            logger.print($"Create session :{sessionI.Id},{serverName} sessions count:{sessions.Count}");
        }

        public override void Shutdown(Ice.Current current)
        {
            current.adapter.getCommunicator().shutdown();

            var logger = current.adapter.getCommunicator().getLogger();
            logger.print("Ice Shutting downed!");
        }

        private void DestroySession(string key)
        {
            SessionI sessionI;
            var ret = sessions.Remove(key, out sessionI);
            if (ret) logger.print($"{sessionI.Id} is destroyed from thread " +
                $"{Thread.CurrentThread.ManagedThreadId},current sessions count:{sessions.Count}.");
        }
    }

}