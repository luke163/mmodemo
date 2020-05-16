using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace TestAOI
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, object> ditc = new Dictionary<string, object>();
            ditc.Add("a", 1);
            ditc.Add("b", "bbb");

            //test(ditc);

        }
        

        //static void test(readonly ref Dictionary<string, object> ditc)
        //{
        //    Console.WriteLine(ditc.Count);

        //    ditc.Add("c", new PaymentService());
        //    Console.WriteLine(ditc.Count);
        //}
    }
}
