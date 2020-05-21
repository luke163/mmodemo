using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using FootStone.FrontOrleans;

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
        internal static Dictionary<string, SessionI> sessions = new Dictionary<string, SessionI>();
        internal static ConcurrentDictionary<string, SessionI> sessionBinds = new ConcurrentDictionary<string, SessionI>();
        
        public static SessionI BindSession(this Ice.Current current, string key, out bool bindsucs)
        {
            bindsucs = false;
            if (!(current.con.getInfo() is Ice.TCPConnectionInfo connection))
            {
                return null;
            }

            var exist = sessions.Remove(connection.connectionId, out var session);
            if (exist)
            {
                bindsucs = sessionBinds.TryAdd(key, session);
                session.Identity = key;
                connection.connectionId = key;
            }

            return session;
        }

        public static SessionI UnbindSession(this Ice.Current current)
        {
            if (!(current.con.getInfo() is Ice.TCPConnectionInfo connection))
            {
                return null;
            }

            sessionBinds.TryRemove(connection.connectionId, out var session);
            connection.connectionId = null;

            return session;
        }

        public static SessionI ObtainSession(this IServantBase servant, Ice.Current current)
        {
            if (servant is null) return null;
            if (!(current.con.getInfo() is Ice.TCPConnectionInfo connection))
            {
                return null;
            }

            sessionBinds.TryGetValue(connection.connectionId, out var session);

            return session;
        }

        public static SessionI ObtainSession(this IObserverBase observer, string key)
        {
            if (observer is null) return null;
            sessionBinds.TryGetValue(key, out var session);

            return session;
        }
    }
}
