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
        private static readonly int ITERATIONS = 1000000 * Environment.ProcessorCount;
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start");
            Console.ReadKey();

            Stopwatch sw = new Stopwatch();
            int[] _arr = new int[ITERATIONS];
            ResetGC();

            #region Synchronic

            Parallel.Invoke(() =>
            {
                sw.Start();
                for (int i = 0; i < ITERATIONS; i++)
                {
                    _arr[i] = i;
                }
                sw.Stop();
            });
            Console.WriteLine("Synchronic: \r\n\tduration = {0:N0}", sw.ElapsedMilliseconds);

            #endregion // Synchronic

            ResetGC();

            #region Task.Factory.StartNew

            Parallel.Invoke(() =>
            {
                sw.Start();
                CountdownEvent countdown = new CountdownEvent(ITERATIONS);
                for (int i = 0; i < ITERATIONS; i++)
                {
                    int local = i;
                    Task.Factory.StartNew(() =>
                        {
                            _arr[local] = local;
                            countdown.Signal();
                        });
                }
                countdown.Wait();
                sw.Stop();
            });
            Console.WriteLine("Task.Factory.StartNew: \r\n\tduration = {0:N0}", sw.ElapsedMilliseconds);

            #endregion // Synchronic

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
                    _arr[i] = i;
                }
            });
            sw.Stop();
            Console.WriteLine("Partitioner: \r\n\tduration = {0:N0}", sw.ElapsedMilliseconds);

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
