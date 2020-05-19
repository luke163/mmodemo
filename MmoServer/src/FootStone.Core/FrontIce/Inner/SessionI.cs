using Ice;
using System;
using System.Collections.Generic;

namespace FootStone.FrontIce
{
    public class SessionI : IDisposable
    {
        private static ulong sss = 0;

        public string Id { get; }
        public ISessionPushPrx SessionPushPrx { get; private set; }
        private Dictionary<string, object> attributes = new Dictionary<string, object>();
        private string bindIdentify;
        private string addressIdentify;

        public SessionI(ISessionPushPrx proxy, string addresskey)
        {
            this.Id = (++sss).ToString();
            this.SessionPushPrx = proxy;
            this.addressIdentify = addresskey;
        }

        public void Destroy()
        {
            SessionPushPrx.ice_getConnection().close(ConnectionClose.Forcefully);
        }

        public T Get<T>(string key)
        {
            if (!attributes.ContainsKey(key))
            {
                return default(T);
            }
            return (T)attributes[key];
        }

        public void Push<T>(string key, T value)
        {
            attributes.Add(key, value);
        }

        public void Unpush(string key)
        {
            attributes.Remove(key);
        }

        public void Bind(string key)
        {
            IceFrontSessionExtensions.sessionBinds.TryAdd(key, addressIdentify);
            bindIdentify = key;
        }

        public void Unbind()
        {
            IceFrontSessionExtensions.sessionBinds.TryRemove(bindIdentify, out _);
            bindIdentify = null;
        }

        public string Identity => bindIdentify;

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
