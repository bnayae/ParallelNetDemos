using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sela.Samples
{
    class Program
    {
        private const int WORK_DURATION_MILLISECONDS = 10000;
        private const int ITERATIONS = 200;

        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();

            for (int i = 0; i < ITERATIONS; i++)
            {
                Console.Write("Q");
                //ThreadPool.QueueUserWorkItem(DoIoLikeWork, null);
                ThreadPool.QueueUserWorkItem(DoCpuWork, null);
            }

            Console.ReadKey();
        }



        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void DoCpuWork(object state)
        {
            Console.Write(".");
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < WORK_DURATION_MILLISECONDS) ;
            Console.Write("*");
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void DoIoLikeWork(object state)
        {
            Console.Write(".");
            Thread.Sleep(WORK_DURATION_MILLISECONDS);
            Console.Write("*");
        }
    }
}
