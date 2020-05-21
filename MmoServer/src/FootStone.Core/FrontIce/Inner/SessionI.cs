using Ice;
using System;
using System.Collections.Generic;

namespace FootStone.FrontIce
{
    public class SessionI : IDisposable
    {
        private static ulong sss = 0;

        public string Id { get; }
        public string Identity { get; set; }
        public ISessionPushPrx SessionPushPrx { get; private set; }
        private Dictionary<string, object> attributes = new Dictionary<string, object>();

        public SessionI(ISessionPushPrx proxy)
        {
            this.Id = $"__{(++sss)}__"; //下滑线防止与绑定标识符撞衫
            this.SessionPushPrx = proxy;
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
