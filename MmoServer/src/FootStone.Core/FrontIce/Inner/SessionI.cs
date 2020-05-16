using Ice;
using System;
using System.Collections.Generic;

namespace FootStone.FrontIce
{
    public class SessionI : IDisposable
    {
        private static ulong sss = 0;

        public String Id { get; }
        public ISessionPushPrx SessionPushPrx { get; private set; }
        private Dictionary<String, object> attributeDict = new Dictionary<string, object>();

        public SessionI(ISessionPushPrx proxy)
        {
            this.Id = (++sss).ToString();
            this.SessionPushPrx = proxy;
        }

        public void Destroy()
        {
            SessionPushPrx.ice_getConnection().close(ConnectionClose.Forcefully);
        }

        public T Get<T>(string key)
        {
            if (!attributeDict.ContainsKey(key))
            {
                return default(T);
            }
            return (T)attributeDict[key];
        }

        public void Bind<T>(string key, T value)
        {
            attributeDict.Add(key, value);
        }

        public void Unbind(string key)
        {
            attributeDict.Remove(key);
        }

        public void Dispose()
        {
            if (SessionPushPrx != null) SessionPushPrx.begin_SessionDestroyed();
        }

        public T UncheckedCastPush<T>(Func<ObjectPrx, string, T> uncheckedCast, string fname) where T : ObjectPrx
        {
            return uncheckedCast(SessionPushPrx, fname);
        }
    }

}
