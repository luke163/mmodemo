using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace FootStone.FrontIce
{
    public static class IceFrontHostingExtensions
    {
        public static IHostBuilder UseFrontIce(this IHostBuilder builder, Action<IceOptions> configureOptions)
        {
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            var key = "IsUseFrontIce";
            if (builder.Properties.ContainsKey(key)) return builder;

            builder.Properties.Add(key, true);
            builder.ConfigureServices(services => {
                services.Configure(configureOptions);
                services.AddSingleton<IHostedService, IceFrontHostedService>();
            });
            return builder;
        }
    }


    public static class IceFrontSessionExtensions
    {
        internal static ConcurrentDictionary<string, SessionI> sessions = new ConcurrentDictionary<string, SessionI>();
        internal static ConcurrentDictionary<string, string> sessionBinds = new ConcurrentDictionary<string, string>();

        public static SessionI ObtainSessionByIPAddress(this Ice.Current current)
        {
            SessionI session = null;
            if (!(current.con.getInfo() is Ice.TCPConnectionInfo connection))
            {
                return session;
            }

            var key = FootStone.Core.HashUnit.GetMd5Str(connection.remoteAddress + ":" + connection.remotePort);
            if (sessions.ContainsKey(key))
            {
                session = sessions[key];
            }

            return session;
        }

        public static SessionI ObtainSessionByBindstr(this IServantBase servant, string key)
        {
            var exist = sessionBinds.TryGetValue(key, out string ipkey);
            //if (!exist) return null;
            exist = sessions.TryGetValue(ipkey, out SessionI session);
            //if (!exist) return null;

            return session;
        }
    }
}
