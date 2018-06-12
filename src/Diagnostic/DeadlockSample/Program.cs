#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion // Using

namespace Sela.Samples
{
    class Program
    {
        private static int _count = 0;
        private static readonly object _sync1 = new object();
        private static readonly object _sync2 = new object();

        static void Main(string[] args)
        {
            Console.WriteLine("Starting");
            // wait until the CPU pressure reduced (Parallel.Invoke is visible to the concurrency profiler)
            Parallel.Invoke(() => Thread.Sleep(2000)); 
            Console.WriteLine("Start");

            Parallel.For(0, 300, A); // schedule 300 tasks

            Console.WriteLine("End");
        }

        private static void A(int i)
        {
            var s = new string('*', (i + 1));
            B(s);
        }

        private static void B(string i)
        {
            int count = Interlocked.Increment(ref _count);
            if (count % 50 == 0) // some tasks will open a new non long running thread
            {
                var t = new Thread(() => DoWork(500));
                t.Name = "MyThread";
                t.IsBackground = true;
                t.Start();
            }

            if (count % 3 == 0)
                C(count);
            else if (count % 3 == 1)
                D(count);
            else
            {
                // some task will open a child task
                Task.Factory.StartNew(ChildRoot, count,
                    TaskCreationOptions.AttachedToParent);
            }
        }

        private static void ChildRoot(object count)
        {
            DoWork(10);
            D((int)count);
        }

        private static void C(int count)
        {
            if (count % 6 == 0)
                E();
            else
            {
                lock (_sync2) // THIS IS WHERE THE DEADLOCK HAPPENS
                {
                    DoWork(10);
                    lock (_sync1) // THIS IS WHERE THE DEADLOCK HAPPENS
                    {
                        D(count);
                    }
                }
            }
        }

        private static void D(int count)
        {
            if (count % 5 == 0)
                E();
            else
            {
                lock (_sync1) // THIS IS WHERE THE DEADLOCK HAPPENS
                {
                    if (count % 2 == 0)
                        E();
                    else
                    {
                        lock (_sync2) // THIS IS WHERE THE DEADLOCK HAPPENS
                        {
                            F();
                        }
                    }
                }
            }
        }

        private static void E()
        {
            DoWork(50);
            Console.Write(".");
        }

        private static void F()
        {
            DoWork(50);
            Console.Write(".");
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static void DoWork(int duration)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < duration) ;
        }
    }

}
