using Ice;
using System;
using System.Threading.Tasks;

namespace FootStone.Client
{
    public interface IFSSession
    {
        string GetId();

        void  Destory();

        void SetDestroyedHandler(EventHandler OnDestroyed);

        T UncheckedCast<T>(Func<ObjectPrx,string,T> uncheckedCast, string name) where T : ObjectPrx;
    }
}
