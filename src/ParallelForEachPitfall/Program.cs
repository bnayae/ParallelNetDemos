using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelForEachPitfall
{
    class Program
    {
        // TODO: EXECUTE IT FROM THE FILE SYSTEM

        static void Main(string[] args)
        {
            Console.WriteLine("Open SysInternals Process Explorer");
            Console.WriteLine("Check Memory and Thread count");
            Console.WriteLine("behavior both when typing and when idle");
            Console.WriteLine("Press any key for start");
            Console.ReadKey(true);
            Console.WriteLine("Start");

            var data = LazyData();
            Parallel.ForEach(data, c => Console.Write("{0},", c));
        }

        private static IEnumerable<char> LazyData()
        {
            while (true)
            {
                yield return Console.ReadKey(true).KeyChar;
            }
        }
    }
}
