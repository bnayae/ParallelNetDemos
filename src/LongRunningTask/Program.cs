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
            var curThread = Thread.CurrentThread;
            Console.WriteLine("Main Thread Id = {0}", curThread.ManagedThreadId);
            Console.WriteLine("Main Is ThreadPool = {0}", curThread.IsThreadPoolThread);

            ParallelUsingThread();
            Thread.Sleep(1500);
            ParallelUsingThreadPool();
            Thread.Sleep(1500);
            ParallelTaskDefault();
            Thread.Sleep(1500);
            ParallelTaskLongRunning();
            Thread.Sleep(1500);
            ParallelTaskSynchronously();
        }

        #region ParallelUsingThread

        private static void ParallelUsingThread()
        {
            Console.WriteLine("\r\n\r\n##################  Thread  ###################");
            var t = new Thread(Exec);
            t.Name = "Background processing";
            t.Priority = ThreadPriority.Normal;
            t.IsBackground = true;

            t.Start(null);
        }

        #endregion // ParallelUsingThread

        #region ParallelUsingThreadPool

        private static void ParallelUsingThreadPool()
        {
            Console.WriteLine("\r\n\r\n##################  ThreadPool  ###################");
            // for this fire and forget overload,
            // there is no built-in way to know
            // when it completed (we will learn how to do it on latter chapters)
            ThreadPool.QueueUserWorkItem(Exec);
        }

        #endregion // ParallelUsingThreadPool

        #region ParallelTaskDefault

        private static void ParallelTaskDefault()
        {
            Console.WriteLine("\r\n\r\n##################  Default Task  ###################");
            var t = new Task(Exec, null);
            t.Start();
        }

        #endregion // ParallelTaskDefault

        #region ParallelTaskLongRunning

        private static void ParallelTaskLongRunning()
        {
            Console.WriteLine("\r\n\r\n##################  Long Running Task  ###################");
            var t = new Task(Exec, null, TaskCreationOptions.LongRunning);
            t.Start();
        }

        #endregion // ParallelTaskLongRunning

        #region ParallelTaskSynchronously

        private static void ParallelTaskSynchronously()
        {
            Console.WriteLine("\r\n\r\n##################  Task Run Synchronously ###################");
            var t = new Task(Exec, null);
            t.RunSynchronously();
        }

        #endregion // ParallelTaskSynchronously

        #region Exec

        public static void Exec(object state)
        {
            var curThread = Thread.CurrentThread;
            Console.WriteLine("Thread Id = {0}", curThread.ManagedThreadId);
            Console.WriteLine("Is ThreadPool = {0}", curThread.IsThreadPoolThread);

            var sw = Stopwatch.StartNew();
            long hang = TimeSpan.FromMilliseconds(50).Ticks;
            while (sw.ElapsedMilliseconds < 1000)
            {
                Console.Write(".");
                while (sw.ElapsedTicks % hang != 0) ;
            }
        }

        #endregion // Exec
    }
}
