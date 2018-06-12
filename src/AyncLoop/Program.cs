#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion // Using

namespace AyncLoop
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = Stopwatch.StartNew();
            Execute();
            Console.WriteLine("Sync: {0:N3}", sw.Elapsed.TotalSeconds);

            Console.ReadKey();
        }

        private static async Task Execute()
        {
            Console.WriteLine("Start");
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(100);
            }

            Console.WriteLine("Async: {0:N3}", sw.Elapsed.TotalSeconds);
        }
    }
}
