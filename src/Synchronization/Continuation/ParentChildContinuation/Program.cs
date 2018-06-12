#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;

#endregion // Using

namespace Tpl.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = Stopwatch.StartNew();
            Task t = Task.Factory.StartNew(state => 
                {
                    Task.Factory.StartNew(() => SpinWork(2000, "real child"),
                        TaskCreationOptions.AttachedToParent);
                    SpinWork(1000, "parent");
                },"Parent");
            t.ContinueWith((tsk) => SpinWork(3000, "not real child"), 
                TaskContinuationOptions.AttachedToParent);
            t.Wait(); // does not wait for the child task
            Console.WriteLine("Done, duration = {0}", sw.ElapsedMilliseconds);
            Console.ReadKey();

        }

        #region SpinWork

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void SpinWork(int duration, string text)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < duration) ; // spin for the duration
            sw.Stop();

            Console.WriteLine(text);
        }

        #endregion // SpinWork
    }
}
