using System.Threading.Tasks;
using FootStone.Client;

namespace FootStone.NetworkNew
{
    public static class www
    {
        public static FSClient client;
		public static FSSession session;
		public static LoggerMobile logger = new LoggerMobile();

        public static void Init()
        {
			FootStone.NLogger.Init(logger);
			
			if (!(client is null)) return;
            client = new FSClient();
            client.IceOptions(iceOptions =>
            {
                iceOptions.EnableDispatcher = false;
                //iceOptions.PushObjects.Add(new PlayerPushI());
                //iceOptions.PushObjects.Add(new ZonePushI());
            });
            client.Start();
        }

        public async static Task<bool> Connect(string ip, int port)
        {
            try
            {
				session = await client.CreateSession(ip, port);
				return true;
            }
            catch (System.Exception e)
            {
                logger.Error("net connect:" + e.ToString());
				return false;
            }
        }
		
		public static void Close()
		{
			client.Stop();
		}
    }
	
	public class LoggerMobile : FootStone.ILogger
    {
        public void Trace(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        public void Debug(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        public void Info(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        public void Warn(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        public void Error(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
        public void Fatal(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }
}
