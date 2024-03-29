﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using System;

namespace FootStone.FrontOrleans
{
    public static class FootStoneHostingExtensions
    {
        public static IHostBuilder UseOrleansClient(this IHostBuilder hostBuilder, Action<IClientBuilder> configureDelegate)
        {
            if (configureDelegate == null) throw new ArgumentNullException(nameof(configureDelegate));

            const string clientBuilderKey = "ClientBuilder";
            if (hostBuilder.Properties.ContainsKey(clientBuilderKey)) return hostBuilder;
            hostBuilder.Properties.Add(clientBuilderKey, true);

            var clientBuilder = new ClientBuilder();
            configureDelegate(clientBuilder);

            hostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<IClientBuilder>(clientBuilder);
                //services.AddSingleton<IHostedService, ClusterClientHostedService>();
                services.AddSingleton<ClusterClientHostedService>();
                services.AddSingleton<IHostedService>(_ => _.GetService<ClusterClientHostedService>());
                services.AddSingleton(_ => _.GetService<ClusterClientHostedService>().Client);
            });

            return hostBuilder;
        }

        public static IHostBuilder UseFrontObserver(this IHostBuilder builder)
        {
            var key = "IsUseFrontObserver";
            if (builder.Properties.ContainsKey(key)) return builder;

            builder.Properties.Add(key, true);
            builder.ConfigureServices(services => {
                services.AddSingleton<IObserverClient>(_ => new ObserverClient(_.GetService<IClusterClient>()));
            });
            return builder;
        }
    }
}
