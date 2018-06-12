using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sela.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");

            ParallelUsingThread();
            //ParallelUsingThreadPool();

            Console.WriteLine("\r\nDone");
            Console.ReadKey();
        }

        #region ParallelUsingThread

        private static void ParallelUsingThread()
        {
            var t = new Thread(Exec);
            t.Name = "Background processing";
            t.Priority = ThreadPriority.Normal;
            t.IsBackground = true;

            t.Start("some optional state");

            while (t.IsAlive)
            {
                Console.Write(".");
                Thread.Sleep(30);
            }
        }

        #endregion // ParallelUsingThread

        #region ParallelUsingThreadPool

        private static void ParallelUsingThreadPool()
        {
            // for this fire and forget overload,
            // there is no built-in way to know
            // when it completed (we will learn how to do it on latter chapters)
            ThreadPool.QueueUserWorkItem(Exec);

            while (true)
            {
                Console.Write(",");
                Thread.Sleep(30);
            }
        }

        #endregion // ParallelUsingThreadPool

        #region Exec

        public static void Exec(object state)
        {
            Console.WriteLine("\r\nThread started with state = {0}\r\n", state);

            var sw = Stopwatch.StartNew();
            long hang = TimeSpan.FromMilliseconds(50).Ticks;
            while (sw.ElapsedMilliseconds < 2000)
            {
                Console.Write("#");
                while (sw.ElapsedTicks % hang != 0) ;
            }
        }

        #endregion // Exec
    }
}
