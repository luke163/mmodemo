using Microsoft.Extensions.Hosting;
using FootStone.FrontIce;
using SampleFrontServer.Handler;
using Sample.Protocol;

namespace FootStone.SampleFrontServer
{
    public static class SampleIceHostingExtensions
    {
        private static void InitIceOptions(IceOptions iceOptions)
        {
            iceOptions.ConfigFile = "Ice.config";
            iceOptions.AddFacetType(typeof(AccountHandler), IAccountPoPrxHelper.ice_staticId());
            //iceOptions.FacetTypes.Add(typeof(WorldI));
            //iceOptions.FacetTypes.Add(typeof(PlayerI));
            //iceOptions.FacetTypes.Add(typeof(RoleMasterI));
            //iceOptions.FacetTypes.Add(typeof(ZoneI));
        }

        public static IHostBuilder UseFrontIce(this IHostBuilder builder)
        {
            builder.UseFrontIce(iceOptions => InitIceOptions(iceOptions));

            return builder;
        }
    }
}
