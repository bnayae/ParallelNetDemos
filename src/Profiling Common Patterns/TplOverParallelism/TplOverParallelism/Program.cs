using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace TplOverParallelism
{
    class Program
    {
        private static readonly int ITERATIONS = 10000000 * Environment.ProcessorCount;
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start");
            Console.ReadKey();

            Stopwatch sw = new Stopwatch();
            long total = 0;
            ResetGC();

		    #region Synchronic

            Parallel.Invoke(() =>
                {
                    sw.Start();
                    for (int i = 0; i < ITERATIONS; i++)
                    {
                        total += i;
                    }
                    sw.Stop();
                });
            Console.WriteLine("Synchronic: \r\n\tduration = {0:N0}, \ttotal = {1}", sw.ElapsedMilliseconds, total);

		    #endregion // Synchronic
                      
            total = 0;
            ResetGC();

            #region Parallel.For

            // sub total with delegate for each iteration
            sw.Restart();
            Parallel.For(0, ITERATIONS,
                () => 0L,
                (i, state, local) => local + i,
                local => total += local);
            sw.Stop();
            Console.WriteLine("Parallel.For: \r\n\tduration = {0:N0},\ttotal = {1}", sw.ElapsedMilliseconds, total);

            #endregion // Parallel.For
            
            total = 0;
            ResetGC();

            #region Partitioner

            // sub total with delegate for each core
            sw.Restart();
            var partitioner = Partitioner.Create(0, ITERATIONS, ITERATIONS / Environment.ProcessorCount);
            Parallel.ForEach(partitioner, tuple =>
                {
                    long localTotal = 0;
                    for (int i = tuple.Item1; i < tuple.Item2; i++)
                    {
                        localTotal += i;
                    }
                    lock (partitioner)
                    {
                        total += localTotal;
                    }
                });
            sw.Stop();
            Console.WriteLine("Partitioner: \r\n\tduration = {0:N0}, \ttotal = {1}", sw.ElapsedMilliseconds, total);

            #endregion // Partitioner

            //Console.ReadKey();
        }

        #region ResetGC

        private static void ResetGC()
        {
            Thread.Sleep(200);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        #endregion // ResetGC
    }
}
