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

/// this sample demonstrate technique of optimizing Parallel.For loop
/// using overload that allow sub calculation

namespace Tpl.Samples
{
    class Program
    {
        private static readonly object _sync = new object();
        private const int SPIN_WORK = 5;
        private const int SIZE = 1000;// 100;
        private const int ITERATIONS = 2;
        private static readonly IEnumerable<int> _range = Enumerable.Range(0, SIZE);

        static void Main(string[] args)
        {
            OptimizedPLinq();
            // warm-up
            Console.WriteLine("warm-up ...");
            NonOptimizedFor();
            OptimizedFor();

            Console.WriteLine("Press any key to start");
            Console.ReadKey();

            #region Synchronic

            Console.ForegroundColor = ConsoleColor.White;
            GC.Collect();
            Console.WriteLine("\r\nSynchronic:");
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < ITERATIONS; i++)
            {
                Synchronic();
                sw.Stop();
            }
            Console.WriteLine("\tduration = {0}", sw.ElapsedMilliseconds);
            Thread.Sleep(500);

            #endregion // Synchronic

            #region NonOptimizedFor

            Console.ForegroundColor = ConsoleColor.Green;
            GC.Collect();
            Console.WriteLine("\r\nNon-Optimized:");
            sw.Restart();
            for (int i = 0; i < ITERATIONS; i++)
            {
                NonOptimizedFor();
                sw.Stop();
            }
            Console.WriteLine("\tduration = {0}", sw.ElapsedMilliseconds);
            Thread.Sleep(500);

            #endregion // NonOptimizedFor

            #region Optimized

            Console.ForegroundColor = ConsoleColor.Yellow;
            GC.Collect();
            Console.WriteLine("\r\nOptimized:");
            sw.Restart();
            for (int i = 0; i < ITERATIONS; i++)
            {
                OptimizedFor();
                sw.Stop();
            }
            Console.WriteLine("\tduration = {0}", sw.ElapsedMilliseconds);

            #endregion // Optimized

            #region Optimized PLinq

            Console.ForegroundColor = ConsoleColor.Yellow;
            GC.Collect();
            Console.WriteLine("\r\nOptimized PLinq:");
            sw.Restart();
            for (int i = 0; i < ITERATIONS; i++)
            {
                OptimizedPLinq();
                sw.Stop();
            }
            Console.WriteLine("\tduration = {0}", sw.ElapsedMilliseconds);

            #endregion // Optimized PLinq

            Console.ReadKey();
        }

        #region Synchronic

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void Synchronic()
        {
            var cancelSource = new CancellationTokenSource();
            var notSafeList = new LinkedList();
            for (int i = 0; i < SIZE; i++)
            {
                lock (_sync)
                {
                    notSafeList.Add(i);
                }
            }
        }

        #endregion // Synchronic

        #region NonOptimizedFor

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void NonOptimizedFor()
        {
            var notSafeList = new LinkedList();
            ParallelLoopResult result = Parallel.For(0, SIZE, 
                i => 
                    {
                        lock (_sync)
                        {
                            notSafeList.Add(i);
                        }
                    });
        }

        #endregion // NonOptimizedFor

        #region OptimizedFor

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void OptimizedFor()
        {
            var notSafeList = new LinkedList();
            #region Documentation
            /*
             * public static ParallelLoopResult For<TLocal>(
             *      int fromInclusive, int toExclusive,
             *      Func<TLocal> localInit,
             *      Func<int, ParallelLoopState, TLocal, TLocal> body,
             *      Action<TLocal> localFinally);
             */
            #endregion // Documentation
            ParallelLoopResult result = Parallel.For<LinkedList>(0, SIZE,
                () => new LinkedList(), // localInit
                (i, state, lst) => { lst.Add(i); return lst; }, // local thread pipeline
                (lst) => // merge all thread local pipelines
                {
                    notSafeList.AddList(lst);
                });
        }

        #endregion // OptimizedFor

        #region OptimizedPLinq

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void OptimizedPLinq()
        {
            #region Documentation
            /*
             * public static TResult Aggregate<TSource, TAccumulate, TResult>(
             *               this ParallelQuery<TSource> source,
             *               Func<TAccumulate> seedFactory,
             *               Func<TAccumulate, TSource, TAccumulate> updateAccumulatorFunc,
             *               Func<TAccumulate, TAccumulate, TAccumulate> combineAccumulatorsFunc,
             *               Func<TAccumulate, TResult> resultSelector);
             */
            #endregion // Documentation
            var notSafeList =_range.AsParallel().Aggregate (
                () => new LinkedList(), // seed
                (lst, i) => { lst.Add(i); return lst; }, // local thread pipeline (accumulate)
                (lst1, lst2) => // merge all thread local pipelines (combine accumulates)
                {
                    lst1.AddList(lst2);
                    return lst1;
                },
                lst => lst); // select
        }

        #endregion // OptimizedPLinq

        #region Nested Types (Linked-list)

        #region LinkedList

        /// <summary>
        /// Non thread-safe linked list
        /// </summary>
        private class LinkedList
        {
            public int Count { get; private set; }
            public LinkedItem First { get; private set; }
            public LinkedItem Last { get; private set; }

            public void Add(int value)
            {
                SpinWork();

                if (First == null)
                {
                    First = new LinkedItem(value);
                    Last = First;
                }
                else
                    Last = Last.Add(value);

                Count++;
            }

            public void AddList(LinkedList lst)
            {
                if (Last == null)
                    Last = lst.First;
                else
                    Last.Next = lst.First;
                Last = lst.Last;

                Count += lst.Count;
            }
        }

        #endregion // LinkedList

        #region LinkedItem

        /// <summary>
        /// Non thread-safe linked list item
        /// </summary>
        private class LinkedItem
        {
            public LinkedItem(int value)
            {
                Value = value;
            }
            public int Value { get; set; }
            public LinkedItem Next { get; set; }
            public LinkedItem Add(int value)
            {
                Next = new LinkedItem(value);
                return Next;
            }
        }

        #endregion // LinkedItem

        #endregion // Nested Types (Linked-list)

        #region SpinWork

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void SpinWork()
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < SPIN_WORK) ; // spin for the duration
            sw.Stop();
        }

        #endregion // SpinWork

    }
}
