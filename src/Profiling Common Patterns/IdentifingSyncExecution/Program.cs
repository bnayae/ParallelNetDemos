#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Threading;

#endregion // Using

namespace Tpl.Samples
{
    class Program
    {
        private const int SPIN_MILLISECONDS = 2000;
        private static readonly object _sync = new object();

        static void Main(string[] args)
        {
            Console.WriteLine("Amdahl's Law");
            Thread.Sleep(2000);

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                Parallel.Invoke(SpinWork);
            }
            
		    #region Hide

            Thread.Sleep(500);

		    #endregion // Hide

            Parallel.For(0, Environment.ProcessorCount, i =>
            {
                lock (_sync)
                    SpinWork();
            });

            Console.WriteLine("Done");
        }

        #region SpinWork

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void SpinWork()
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < SPIN_MILLISECONDS) ; // spin for the duration
            sw.Stop();
        }

        #endregion // SpinWork
    }
}
