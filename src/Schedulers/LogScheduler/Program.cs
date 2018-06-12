#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Runtime;

#endregion // Using

namespace Tpl.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            var scheduler = new CustomLogScheduler(TaskScheduler.Default);
            Parallel.For(0, 2, new ParallelOptions { TaskScheduler = scheduler }, i => Console.WriteLine(i));
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (TimeSpan item in scheduler)
            {
                Console.WriteLine(item.Ticks);
            }
            Console.ReadKey();
        }
    }
}
