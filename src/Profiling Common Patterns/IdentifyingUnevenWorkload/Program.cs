#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;

#endregion // Using

namespace Tpl.Samples
{
    class Program
    {
        private static readonly int ITERATIONS = 10 * Environment.ProcessorCount;
        private const int WORK_FACTOR = 100;

        static void Main(string[] args)
        {
            Console.WriteLine("Static partitioning doesn't optimized for Uneven workload");
            Console.WriteLine("Check CPU and Thread views");
            Thread.Sleep(4000);
            var sw = Stopwatch.StartNew();

            var staticRangePartitioner = Partitioner.Create(0, ITERATIONS, ITERATIONS / Environment.ProcessorCount);
            Parallel.ForEach(staticRangePartitioner, (range, state, partitionIndex) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    SpinWork(i);
                }
            });

            Console.WriteLine("Duration = {0}", sw.ElapsedMilliseconds);
        }

        #region SpinWork

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void SpinWork(int i)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < (i * WORK_FACTOR)) ; // spin for the duration
            sw.Stop();
        }

        #endregion // SpinWork
    }
}
