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

#endregion // Using

namespace Tpl.Samples
{
    class Program
    {
        private const int SPIN_WORK_DURATION = 100;

        static void Main(string[] args)
        {
            #region Hide

            Console.SetWindowSize(Console.WindowWidth, 48);
            ConcurrentQueue<Action> _writeLineActions = new ConcurrentQueue<Action>(); // used to avoid Console.WriteLine before everything had completed
            var cancelSouce = new CancellationTokenSource();

            #endregion // Hide

            // Define scheduler that schedule work on lowest thread priority
            var scheduler = new CustomPriorityScheduler(
                ThreadPriority.Lowest,
                "My Thread",
                Environment.ProcessorCount);

            #region Hide

            _writeLineActions.Enqueue(() => Console.WriteLine("\r\nTask.Factory.StartNew \r\n"));

            #endregion // Hide

            #region Actions

            Action action = () => _writeLineActions.Enqueue(WriteThreadInfo());
            Action actionSingleTread = () =>
            {
                _writeLineActions.Enqueue(() => Console.ForegroundColor = ConsoleColor.Blue);
                action();
                _writeLineActions.Enqueue(() => Console.ResetColor());
            };
            Action actionHighest = () =>
            {
                _writeLineActions.Enqueue(() => Console.ForegroundColor = ConsoleColor.Red);
                action();
                _writeLineActions.Enqueue(() => Console.ResetColor());
            };

            #endregion // Actions

            // schedule task on lowest priority
            Task tCustom = Task.Factory.StartNew(action, cancelSouce.Token, TaskCreationOptions.None, scheduler);
            // schedule task on the default scheduler
            Task t = Task.Factory.StartNew(action);
            Task.WaitAll(tCustom, t);

            #region Hide

            _writeLineActions.Enqueue(() => Console.WriteLine("\r\n------------------------------\r\n"));
            _writeLineActions.Enqueue(() => Console.ForegroundColor = ConsoleColor.White);
            _writeLineActions.Enqueue(() => Console.WriteLine("\r\nParallel.For \r\n"));

            #endregion // Hide

            // schedule parallel for on lowest priority 
            // even thou the scheduler doesn't limit the amount of threads, 
            // having MaxDegreeOfParallelism = 1 will schedule the parallel for on single thread
            var options = new ParallelOptions { TaskScheduler = scheduler };//, MaxDegreeOfParallelism = 1 };
            Parallel.For(1, 10, options, i => _writeLineActions.Enqueue(WriteThreadInfo()));

            #region Hide

            _writeLineActions.Enqueue(() => Console.WriteLine("\r\n------------------------------\r\n"));
            _writeLineActions.Enqueue(() => Console.ResetColor());
            _writeLineActions.Enqueue(() => Console.WriteLine("\r\nfor (Task.Factory.StartNew) \r\n"));

            #endregion // Hide

            // Define scheduler that schedule work on lowest thread priority and do the work on single thread
            var schedulerSingleTread = new CustomPriorityScheduler(
                ThreadPriority.Lowest,
                "My Thread Single Thread",
                1);

            // Define scheduler that schedule work on highest thread priority
            var schedulerHighest = new CustomPriorityScheduler(
                ThreadPriority.Highest,
                "My Thread Highest Thread",
                Environment.ProcessorCount);

            List<Task> waitingList = new List<Task>();
            for (int i = 0; i < 10; i++) // race
            {
                // schedule task on lowest priority and single thread
                tCustom = Task.Factory.StartNew(actionSingleTread, cancelSouce.Token, TaskCreationOptions.None, schedulerSingleTread);
                waitingList.Add(tCustom);

                // schedule task on highest priority
                tCustom = Task.Factory.StartNew(actionHighest, cancelSouce.Token, TaskCreationOptions.None, schedulerHighest);
                waitingList.Add(tCustom);

                // schedule task on the default scheduler
                t = Task.Factory.StartNew(action);
                waitingList.Add(t);
            }
            Task.WaitAll(waitingList.ToArray());

            #region Hide

            foreach (Action write in _writeLineActions)
            {
                write();
            }

            #endregion // Hide

            Console.ReadKey();
        }

        #region SpinWork

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SpinWork(int milliseconds)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < milliseconds) ; // spin for the duration
            sw.Stop();
        }

        #endregion // SpinWork

        #region WriteThreadInfo

        /// <summary>
        /// Get the current thread info
        /// spin work
        /// and return action for writing the thread info (at latter time)
        /// </summary>
        /// <returns></returns>
        private static Action WriteThreadInfo()
        {
            string s = string.Format("Name = {0}, Priority = {1}, Is pool = {2}, id = {3}",
                Thread.CurrentThread.Name,
                Thread.CurrentThread.Priority,
                Thread.CurrentThread.IsThreadPoolThread,
                Thread.CurrentThread.ManagedThreadId);

            SpinWork(SPIN_WORK_DURATION);
            return () => Console.WriteLine(s);
        }

        #endregion // WriteThreadInfo
    }
}
