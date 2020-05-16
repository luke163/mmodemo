using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using System;

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
                // this configures the test running on this particular client
                services.Configure(configureOptions);
                // this hosted service runs the sample logic
                services.AddSingleton<IHostedService, IceFrontHostedService>();
            });
            return builder;
        }
    }
}
