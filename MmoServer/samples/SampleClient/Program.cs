using System;
using System.Threading.Tasks;

namespace SampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                FootStone.NLogger.Init(new LoggerClt());

                int count = args.Length > 0 ? int.Parse(args[0]) : 1;
                ushort startIndex = args.Length > 1 ? ushort.Parse(args[1]) : (ushort)0;
                bool needNetty = args.Length > 2 ? bool.Parse(args[2]) : true;

                string ip = args.Length > 3 ?args[3] : "127.0.0.1";
                int port = args.Length > 4 ? int.Parse(args[4]) : 4061;

                //OldTest(count, startIndex, needNetty);
                NewTest(ip,port,count, startIndex, needNetty).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async Task NewTest(string ip,int port,int count, ushort startIndex, bool needNetty)
        {
            var network = new NetworkNew();
            await network.Test(ip,port,count, startIndex, needNetty);
        }


        static void OldTest(int count, ushort startIndex, bool needNetty)
        {
            //var network = new Network();
            //network.Test(count, startIndex, needNetty).Wait();
         
        }
    }
}
