using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FootStone.FrontIce
{
    class FSInterceptor : Ice.DispatchInterceptor, IDisposable
    {
        private Ice.Object servant;
        private Ice.Logger logger;

        public FSInterceptor(Ice.Object servant, Ice.Logger logger)
        {
            this.servant = servant;
            this.logger = logger;
        }

        public async override Task<Ice.OutputStream> dispatch(Ice.Request request)
        {
            try
            {
                var ret = await servant.ice_dispatch(request);
                return ret;
            }
            catch (Ice.Exception e)
            {
                logger.trace("FSInterceptor", e.ToString());
                throw e;
            }
            catch (System.Exception e)
            {
                logger.error(e.ToString());
                throw e;
            }
        }

        public void Dispose()
        {
            IDisposable dis = servant as IDisposable;
            if (dis != null) dis.Dispose();
        }

    }
}
