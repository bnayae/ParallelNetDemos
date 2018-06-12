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
        private const int WORK_LEVEL = 1000; //10000;
        private const int ITERATIONS = 5000; //50000;
        private static readonly CountdownEvent _cd = new CountdownEvent(ITERATIONS);

        static void Main(string[] args)
        {

            Console.ForegroundColor = ConsoleColor.White;
            #region Warm-up

            Thread t = new Thread(() => { });
            t.Start();
            ThreadPool.QueueUserWorkItem(state => { }, null);
            Thread.Sleep(200);

            #endregion // Warm-up

            for (int i = 0; i < 3; i++)
            {
                Measure(ExecPool, "ExecPool");
                Measure(ExecThread, "ExecThread");
            }
        }

        #region Measure

        private static void Measure(Action operation, string title)
        {
            _cd.Reset();
            Thread.Sleep(100);
            var sw = Stopwatch.StartNew();

            operation();

            _cd.Wait();
            sw.Stop();
            Console.WriteLine("\r\n{0}: Duration = {1:N}", title, sw.ElapsedMilliseconds);
        }

        #endregion // Measure

        #region ExecPool

        private static void ExecPool()
        {
            for (int i = 0; i < ITERATIONS; i++)
            {
                ThreadPool.QueueUserWorkItem(UnitOfWork, null);
            }
        }

        #endregion // ExecPool

        #region ExecThread

        private static void ExecThread()
        {
            for (int i = 0; i < ITERATIONS; i++)
            {
                Thread t = new Thread(UnitOfWork);
                t.IsBackground = true;
                t.Name = "T " + i;
                t.Start(null);
            }
        }

        #endregion // ExecThread

        #region UnitOfWork

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void UnitOfWork(object state)
        {
            for (int i = 0; i < WORK_LEVEL; i++)
            {
            }
            _cd.Signal();
        }

        #endregion // UnitOfWork
    }
}
