#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;

#endregion // Using

namespace SemaphorePerfCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Create

            //PerformanceCounterCategory.Delete("TplSample");
            if (!PerformanceCounterCategory.Exists("TplSample"))
            {
                CounterCreationDataCollection counters = new CounterCreationDataCollection();

                // 1. counter for counting totals: PerformanceCounterType.NumberOfItems32
                CounterCreationData totalOps = new CounterCreationData();
                totalOps.CounterName = "TplSemaphoreCounter";
                totalOps.CounterType = PerformanceCounterType.NumberOfItems32;
                counters.Add(totalOps);

                // create new category with the counters above
                PerformanceCounterCategory.Create("TplSample", "Tpl course counter",
                    PerformanceCounterCategoryType.SingleInstance, counters);
            }

            PerformanceCounter pc = new PerformanceCounter("TplSample", "TplSemaphoreCounter", false);

            #endregion // Create

            Console.WriteLine("Open Perfmon and listen to the TempCounter, \r\nthan press any key to continue");
            Console.ReadKey();

            const int ITERATIONS = 100;
            int maxCount = 0;
            using (var semaphore = new SemaphoreSlim(10, 10))
            using (var countdown = new CountdownEvent(ITERATIONS))
            {
                object sync = new object();
                int count = 0;
                for (int i = 0; i < ITERATIONS; i++)
                {
                    new Thread(() =>
                        {
                            semaphore.Wait();
                            Interlocked.Increment(ref count);
                            Console.Write(".");

                            pc.Increment();
                            Thread.Sleep(300);

                            lock (sync)
                            {
                                if (maxCount < count)
                                    maxCount = count;
                            }
                            Interlocked.Decrement(ref count);
                            pc.Decrement();
                            semaphore.Release();
                            countdown.Signal();
                        }).Start(); // create new threads for every iteration
                }
                countdown.Wait();
            } // semaphore.Close();

            Console.WriteLine("\r\nDone {0}", maxCount);
            Console.ReadKey();
        }

        #region SpinWork

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void SpinWork(int duration)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < duration) ; // spin for the duration
            sw.Stop();
        }

        #endregion // SpinWork
    }
}
