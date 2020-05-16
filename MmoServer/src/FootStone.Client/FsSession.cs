using Ice;
using System;

namespace FootStone.Client
{
    public class FSSession : IFSSession
    {
        private string id;
        private SessionIce session;
        //public event EventHandler OnDestroyed;

        public FSSession(string id, SessionIce session)
        {
            this.id = id;
            this.session = session;
        }

        public void SetDestroyedHandler(EventHandler OnDestroyed)
        {
            this.session.SessionPush.OnDestroyed += OnDestroyed;
        }

        public void Destory()
        {
            session.Destroy();
        }

        public string GetId()
        {
            return id;
        }

        public T UncheckedCast<T>(Func<ObjectPrx, string, T> uncheckedCast, string name) where T : ObjectPrx
        {
            return uncheckedCast(session.SessionPrx, name);
        }
    }
}
