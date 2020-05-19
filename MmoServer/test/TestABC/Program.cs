using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestAOI
{
    class Program
    {
        async static Task Main(string[] args)
        {
            DisplayDefaultOf<PaymentService>();

            DisplayDefaultOf<Guid>();

            Console.ReadLine();
        }

        static void DisplayDefaultOf<T>()
        {
            var val = default(T);
            Console.WriteLine($"Default value of {typeof(T)} is {(val == null ? "null" : val.ToString())}.");
        }


        static Task<string> test(string abc)
        {
            return Task.FromResult(abc);
        }
    }
}
