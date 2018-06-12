#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion // Using

namespace ThreadPoolBehavior
{
    enum Mode
    { 
        ThreadPool,
        Task,
        FairTask
    }

    class Program
    {
        // TODO: Change the modes
        private const Mode MODE = Mode.ThreadPool; 
        
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            int half = Environment.ProcessorCount / 2;

            for (int i = 0; i < half; i++)
            {
                int id = i;
                Invoke(() => DoComplexWork(id));
            }
            Thread.Sleep(1);
            for (int i = half; i < Environment.ProcessorCount + half; i++)
            {
                int id = i;
                Invoke(() => DoWork("Parent " + i, 1000));
            }

            Console.ReadKey();
        }

        private static void Invoke(Action action)
        {
            switch (MODE)
            {
                case Mode.ThreadPool:
                    ThreadPool.QueueUserWorkItem(state => action());
                    break;
                case Mode.Task:
                    Task.Run(action);
                    break;
                case Mode.FairTask:
                    Task.Factory.StartNew(action, TaskCreationOptions.PreferFairness);
                    break;
            }
        }

        #region DoComplexWork

        private static void DoComplexWork(int i)
        {
            DoWork("Parent " + i, 500);
            Invoke(() => DoWork("\tChild: " + i, 1000));
        }

        #endregion // DoComplexWork

        #region DoWork

        private static void DoWork(string title, int duration)
        {
            WriteInfo(title);
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < duration); 
        }

        #endregion // DoWork

        #region WriteInfo

        private static void WriteInfo(string title)
        {
            var t = Thread.CurrentThread;
            Console.WriteLine("{0}, Thread ID = {1}", title, t.ManagedThreadId);
        }

        #endregion // WriteInfo
    }
}
