using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CacheLineSample
{
    class Program
    {
        private const int MAX = 10000000;
        private const int THREAD_COUNT = 10;

        private static int[] _arr;
        
        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(THREAD_COUNT * 2, 2);
            _arr = Enumerable.Range(0, MAX).ToArray();
            // warm-up
            RunAsync(10);
            RunPartitionAsync(10);

            // benchmark
            Console.WriteLine("Start");

            for (int i = 0; i < 3; i++)
            {
                Benchmark();
                Console.WriteLine();
            }

            Console.ReadKey();
        }

        private static void Benchmark()
        {
            var sw = new Stopwatch();
            sw.Start();
            long total = Run(MAX, 0, 1);
            sw.Stop();
            Console.WriteLine("Synchronic: Duration = {0}, Total = {1:N0}", sw.ElapsedMilliseconds, total);

            sw.Reset();
            sw.Start();
            total = RunAsync(MAX);
            sw.Stop();
            Console.WriteLine("Async: Duration = {0}, Total = {1:N0}", sw.ElapsedMilliseconds, total);

            sw.Reset();
            sw.Start();
            total = RunPartitionAsync(MAX);
            sw.Stop();
            Console.WriteLine("Partition Async: Duration = {0}, Total = {1:N0}", sw.ElapsedMilliseconds, total);
        }


        private static long RunAsync(int max)
        {
            long[] sum = new long[THREAD_COUNT];
            using (var cd = new CountdownEvent(THREAD_COUNT))
            {
                for (int i = 0; i < THREAD_COUNT; i++)
                {
                    ThreadPool.QueueUserWorkItem(state =>
                        {
                            int seed = (int)state;
                            long total = Run(max, seed, THREAD_COUNT);
                            sum[seed] = total;
                            cd.Signal();
                        }, i);
                }
                cd.Wait();
            }

            return sum.Sum();
        }

        private static long RunPartitionAsync(int max)
        {
            long[] sum = new long[THREAD_COUNT];
            int partitionSize = max / THREAD_COUNT;
            using (var cd = new CountdownEvent(THREAD_COUNT))
            {
                for (int i = 0; i < THREAD_COUNT; i++)
                {
                    int lbound = partitionSize * i;
                    int ubound = lbound + partitionSize;
                    ThreadPool.QueueUserWorkItem(state =>
                        {
                            int index = (int)state;
                            long total = Run(ubound, lbound, 1);
                            sum[index] = total;
                            cd.Signal();
                        }, i);
                }
                cd.Wait();
            }

            return sum.Sum();
        }

        private static long Run(int max, int seed, int step)
        {
            long sum = 0;
            for (int i = seed; i < max; i+=step)
            {
                sum += _arr[i];
            }
            return sum;
        }
    }
}
