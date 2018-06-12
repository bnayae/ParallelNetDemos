#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Threading;
using System.Collections.Concurrent;

#endregion // Using

/// demonstrate the different partitioning behavior (for balanced and unbalanced collections)
/// which depend on whether the collection is index-able or not

namespace Tpl.Samples
{
    class Program
    {
        private const int BASE_DURATION = 10;
        private const int ITERATIONS_PER_CORE = 20;
        private static readonly ConsoleColor[] _colors =
            new[] { ConsoleColor.White, ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Magenta, ConsoleColor.DarkYellow, ConsoleColor.Blue, ConsoleColor.Red, ConsoleColor.DarkGreen };

        static void Main(string[] args)
        {
            IEnumerable<Info> data = CreateData(UnbalanceDurationStrategy);
            var unbalanceDataNonIndexable = new Queue<Info>(data);  // Non index-able collection
            var unbalanceDataIndexable = data.ToArray();            // index-able collection
            data = CreateData(BalanceDurationStrategy);
            var balanceDataNonIndexable = new Queue<Info>(data);    // Non index-able collection
            var balanceDataIndexable = data.ToArray();              // index-able collection

            Console.WriteLine("\r\nCurrent Thread={0}\r\n", Thread.CurrentThread.ManagedThreadId);

            char c;
            do
            {
                #region Choose Options

                Console.WriteLine("0. Unbalance chunk partitions");
                Console.WriteLine("1. Unbalance range partitions");
                Console.WriteLine("2. Balance chunk partitions");
                Console.WriteLine("3. Balance range partitions");
                Console.WriteLine("9. All");
                Console.WriteLine("q. Quit");

                #endregion // Choose Options

                c = Console.ReadKey().KeyChar;
                switch (c)
                {
                    case '0':
                        Parallel.ForEach(unbalanceDataNonIndexable, ExecuteItem); // Warm-up
                        Thread.Sleep(1000);
                        Execute(unbalanceDataNonIndexable, "Unbalance chunk partitions");
                        break;
                    case '1':
                        Parallel.ForEach(unbalanceDataIndexable, ExecuteItem); // Warm-up
                        Thread.Sleep(1000);
                        Execute(unbalanceDataIndexable, "Unbalance range partitions");
                        break;

                    case '2':
                        Parallel.ForEach(balanceDataNonIndexable, ExecuteItem); // Warm-up
                        Thread.Sleep(1000);
                        Execute(balanceDataNonIndexable, "Balance chunk partitions");
                        break;
                    case '3':
                        Parallel.ForEach(balanceDataIndexable, ExecuteItem); // Warm-up
                        Thread.Sleep(1000);
                        Execute(balanceDataIndexable, "Balance range partitions");
                        break;
                    case '9':
                        #region All

                        Parallel.ForEach(unbalanceDataNonIndexable, ExecuteItem); // Warm-up
                        Parallel.ForEach(unbalanceDataIndexable, ExecuteItem); // Warm-up
                        Parallel.ForEach(balanceDataNonIndexable, ExecuteItem); // Warm-up
                        Parallel.ForEach(balanceDataIndexable, ExecuteItem); // Warm-up
                        Thread.Sleep(1000);

                        Execute(unbalanceDataNonIndexable, "Unbalance chunk partitions");
                        Execute(unbalanceDataIndexable, "Unbalance range partitions");
                        Execute(balanceDataNonIndexable, "Balance chunk partitions");
                        Execute(balanceDataIndexable, "Balance range partitions");

                        #endregion // All
                        break;
                    default:
                        return;
                }

                Console.WriteLine("Done");
            } while (true);
        }

        #region CreateData

        private static IEnumerable<Info> CreateData(Func<int, int, int> DurationStrategy)
        {
            var range = Enumerable.Range(0, Environment.ProcessorCount * ITERATIONS_PER_CORE);
            var execData = from i in range
                           let duration = DurationStrategy(range.Count(), i)
                           select new Info(i, duration);
            return execData;
        }

        #endregion // CreateData

        #region UnbalanceDurationStrategy

        private static int UnbalanceDurationStrategy(int count, int index)
        {
            int partitionSize = count / Environment.ProcessorCount;
            int partitionNumber = ((index / partitionSize) + 1);
            int sleepDuration = BASE_DURATION * partitionNumber * partitionNumber;
            return sleepDuration;
        }

        #endregion // UnbalanceDurationStrategy

        #region BalanceDurationStrategy

        private static int BalanceDurationStrategy(int count, int index)
        {
            return BASE_DURATION * 3;
        }

        #endregion // BalanceDurationStrategy

        #region Execute

        private static void Execute(IEnumerable<Info> collection, string title)
        {
            var sw = Stopwatch.StartNew();
            Parallel.ForEach(collection, ExecuteItem);
            sw.Stop();
            Console.WriteLine("\r\n---------------------------------------------------------------------");
            Console.WriteLine("\r\n{0}: Duration = {1}\r\n", title, sw.ElapsedMilliseconds);
            WriteInfo(collection);
            Thread.Sleep(500);
        }

        private static void ExecuteItem(Info info)
        {
            info.ThreadId = Thread.CurrentThread.ManagedThreadId;
            info.Ticks = Environment.TickCount;
            Thread.Sleep(info.Duration);
        }

        #endregion // Execute

        #region SpinWork

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SpinWork(int milliseconds)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < milliseconds) ; // spin for the duration
            sw.Stop();
        }

        #endregion // SpinWork

        #region WriteInfo

        private static void WriteInfo(IEnumerable<Info> executionData)
        {
            executionData = from item in executionData
                            //orderby item.ThreadId, item.Index
                            orderby item.ThreadId, item.Ticks
                            select item;

            foreach (Info info in executionData)
            {
                int threadId = info.ThreadId;
                ConsoleColor color = _colors[threadId % 8];
                Console.ForegroundColor = color;
                Console.WriteLine("Index={0}, Thread={1}, Duration={2}", info.Index, threadId, info.Duration);
                Console.ResetColor();
            }
        }

        #endregion // WriteInfo

        #region Info

        private class Info
        {
            public Info(int index, int duration)
            {
                Index = index;
                Duration = duration;
            }

            public int ThreadId { get; set; }
            public int Index { get; private set; }
            public int Duration { get; private set; }
            public int Ticks { get; set; }
        }

        #endregion // Info
    }
}
