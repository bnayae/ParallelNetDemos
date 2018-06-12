#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;

#endregion // Using

namespace Tpl.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            TaskScheduler scheduler = new LowPriorityScheduler();

            for (int i = 0; i < 100; i++)
            {
                Task.Factory.StartNew(() => Console.WriteLine(Thread.CurrentThread.Priority));
                Task.Factory.StartNew(() => Console.WriteLine("   " + Thread.CurrentThread.Priority), 
                    CancellationToken.None, TaskCreationOptions.None, scheduler);
            }

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
