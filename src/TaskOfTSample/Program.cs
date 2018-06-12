using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sela.Sample
{
    class Program
    {
        private const int ITERATIONS = 10;

        static void Main(string[] args)
        {
            Console.WriteLine("Start");

            var tasks = new Task<long>[ITERATIONS];
            for (int i = 0; i < ITERATIONS; i++)
            {
                tasks[i] = Task<long>.Factory.StartNew(Exec, i);
            }

            // t.Result is blocking (therefore Sum will wait to the completion of all task)
            Console.WriteLine("\r\nTotal = {0}", tasks.Sum(t => t.Result));
            Console.WriteLine("Notice that some task started with delay (ThreadPool behavior)");
            Console.ReadKey();
        }

        #region Exec

        public static long Exec(object state)
        {
            Console.WriteLine("\r\nThread started with state = {0}\r\n", state);

            var sw = Stopwatch.StartNew();
            long hang = TimeSpan.FromMilliseconds(50).Ticks;
            while (sw.ElapsedMilliseconds < 2000)
            {
                Console.Write("#");
                while (sw.ElapsedTicks % hang != 0) ;
            }

            return sw.ElapsedTicks & 100;
        }

        #endregion // Exec    }
    }
}
