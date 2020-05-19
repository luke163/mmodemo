using System;
using System.Threading.Tasks;
using Orleans;

namespace FootStone.FrontOrleans
{
    public interface IObserverClient
    {
        public Task<string> GetObserver<TObserver, IObserver>(Guid primaryKey) 
            where TObserver : class, IObserver, new()
            where IObserver : IGrainObserver;

        public Task<string> GetObserver<TObserver, IObserver>(long primaryKey) 
            where TObserver : class, IObserver, new()
            where IObserver : IGrainObserver;

        public Task<string> GetObserver<TObserver, IObserver>(string primaryKey) 
            where TObserver : class, IObserver, new()
            where IObserver : IGrainObserver;

        public Task<string> GetObserver<TObserver, IObserver>(Guid primaryKey, string keyExtension) 
            where TObserver : class, IObserver, new()
            where IObserver : IGrainObserver;

        public Task<string> GetObserver<TObserver, IObserver>(long primaryKey, string keyExtension) 
            where TObserver : class, IObserver, new()
            where IObserver : IGrainObserver;

        uint SubscribeWrap<IObserver>(string key, SubscribeCallback<IObserver> callback)
            where IObserver : IGrainObserver;

        uint UnsubscribeWrap<IObserver>(string key, SubscribeCallback<IObserver> callback)
            where IObserver : IGrainObserver;
    }

    public delegate void SubscribeCallback<IObserver>(IObserver reference) where IObserver : IGrainObserver;
}
