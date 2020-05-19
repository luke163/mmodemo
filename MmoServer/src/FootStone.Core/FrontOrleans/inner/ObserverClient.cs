using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Orleans;

namespace FootStone.FrontOrleans
{
	public class ObserverClient : IObserverClient
	{
		class ObserverInfo
		{
			public IGrainObserver value;
			public IGrainObserver reference;
			public uint cc;
		}

		private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
		private Dictionary<string, ObserverInfo> observers = new Dictionary<string, ObserverInfo>();
		private IClusterClient cluster;


		public ObserverClient(IClusterClient clus)
		{
			cluster = clus;
		}
		
		public async Task<string> GetObserver<TObserver, IObserver>(Guid primaryKey) 
			where TObserver : class, IObserver, new()
			where IObserver : IGrainObserver
		{
			string key = $"{nameof(TObserver)}^guid^{primaryKey}";
			key = FootStone.Core.HashUnit.GetMd5Str(key);
			await this.GetOrAdd<TObserver, IObserver>(key);

			return key;
		}
		
		public async Task<string> GetObserver<TObserver, IObserver>(long primaryKey) 
			where TObserver : class, IObserver, new()
			where IObserver : IGrainObserver
		{
			string key = $"{nameof(TObserver)}^integer^{primaryKey}";
			key = FootStone.Core.HashUnit.GetMd5Str(key);
			await this.GetOrAdd<TObserver, IObserver>(key);

			return key;
		}
		
		public async Task<string> GetObserver<TObserver, IObserver>(string primaryKey) 
			where TObserver : class, IObserver, new()
			where IObserver : IGrainObserver
		{
			string key = $"{nameof(TObserver)}^string^{primaryKey}";
			key = FootStone.Core.HashUnit.GetMd5Str(key);
			await this.GetOrAdd<TObserver, IObserver>(key);

			return key;
		}
		
		public async Task<string> GetObserver<TObserver, IObserver>(Guid primaryKey, string keyExtension) 
			where TObserver : class, IObserver, new()
			where IObserver : IGrainObserver
		{
			string key = $"{nameof(TObserver)}^guid^string^{primaryKey}^{keyExtension}";
			key = FootStone.Core.HashUnit.GetMd5Str(key);
			await this.GetOrAdd<TObserver, IObserver>(key);

			return key;
		}
		
		public async Task<string> GetObserver<TObserver, IObserver>(long primaryKey, string keyExtension) 
			where TObserver : class, IObserver, new()
			where IObserver : IGrainObserver
		{
			string key = $"{nameof(TObserver)}^integer^string^{primaryKey}^{keyExtension}";
			key = FootStone.Core.HashUnit.GetMd5Str(key);
			await this.GetOrAdd<TObserver, IObserver>(key);

			return key;
		}

		private async Task<ObserverInfo> GetOrAdd<TObserver, IObserver>(string key) 
			where TObserver : class, IObserver, new()
			where IObserver : IGrainObserver
		{
			ObserverInfo info;
			if (this.observers.ContainsKey(key))
			{
				info = this.observers[key];
			}
			else
			{
				TObserver obsvr = (TObserver)Activator.CreateInstance(typeof(TObserver));
				IObserver refer = await cluster.CreateObjectReference<IObserver>(obsvr);
				info = new ObserverInfo() { value = obsvr, reference = refer, cc = 0 };
				this.observers.Add(key, info);
			}

			return info;
		}

		public uint SubscribeWrap<IObserver>(string key, SubscribeCallback<IObserver> callback)
			where IObserver : IGrainObserver
		{
			if (!observers.ContainsKey(key))
			{
				logger.Error($"{key}don't find observer instance.({nameof(IObserver)})");
				return uint.MaxValue;
			}

			ObserverInfo info = observers[key];
			if (!(info.reference is IObserver))
			{
				logger.Error($"Type of observer isn't {nameof(IObserver)}.");
				return uint.MaxValue;
			}

			++info.cc;
			callback.Invoke((IObserver)info.reference);

			return info.cc;
		}

		public uint UnsubscribeWrap<IObserver>(string key, SubscribeCallback<IObserver> callback)
			where IObserver : IGrainObserver
		{
			if (!observers.ContainsKey(key))
			{
				logger.Error($"{key}don't find observer instance.({nameof(IObserver)})");
				return uint.MaxValue;
			}

			ObserverInfo info = observers[key];
			if (!(info.reference is IObserver))
			{
				logger.Error($"Type of observer isn't {nameof(IObserver)}.");
				return uint.MaxValue;
			}

			--info.cc;
			if (info.cc <= 0) callback.Invoke((IObserver)info.reference);

			return info.cc;
		}
	}
}
