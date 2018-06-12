#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics;

#endregion // Using

/// this sample demonstrate Sum of tree nodes values
/// the sum method is implement under different strategies:
/// - Synchronic
/// - Task (Parallel)
/// - APM (BeginInvoke / EndInvoke)
/// It demonstrate how much easier is to get a-sync result using the Task<T> than by APM
/// 
/// focus on the Task<T> vs. the APM implementation

namespace Tpl.Samples
{
    class Program
    {
        #region Constants

        public const int SPIN_MILLISECONDS = 10;
        private const int STRATEGY_SWITCH_SLEEP = 2000;
        private const int REPEAT_SLEEP = 500;
        private const int REPEATS = 3;
        private const int RANGE_SIZE = 60; // 5000;

        #endregion // Constants

        private static Node _root;

        static void Main(string[] args)
        {
            Warmup();

            CreateTree(RANGE_SIZE);

            Console.WriteLine("Size = {0}", RANGE_SIZE);

            while (true)
            {
                #region List<KeyValuePair<string, IStrategy>> strategies = ...

                Console.WriteLine("Choose strategy:");
                Console.WriteLine("0. Synchronic");
                Console.WriteLine("1. Parallel Task");
                Console.WriteLine("2. BeginInvoke");
                Console.WriteLine("9. All but Thread");
                Console.WriteLine("q. quit");

                Console.ForegroundColor = ConsoleColor.Black;
                var selection = Console.ReadKey().KeyChar;
                Console.ResetColor();
                Console.WriteLine();

                var strategies = new List<KeyValuePair<string, IStrategy>>();

                if (selection == '0' || selection == '9')
                {
                    var infoSynchronic = new KeyValuePair<string, IStrategy>("Synchronic:", new SynchronicStrategy());
                    strategies.Add(infoSynchronic);
                }
                if (selection == '1' || selection == '9')
                {
                    var infoTask = new KeyValuePair<string, IStrategy>("Parallel Task:", new ParallelTaskStrategy());
                    strategies.Add(infoTask);
                }
                if (selection == '2' || selection == '9')
                {
                    var infoBeginInvoke = new KeyValuePair<string, IStrategy>("BeginInvoke:", new BeginInvokeStrategy());
                    strategies.Add(infoBeginInvoke);
                }

                if (strategies.Count == 0)
                    return;

                #endregion // List<KeyValuePair<string, IStrategy>> strategies = ...

                foreach (KeyValuePair<string, IStrategy> item in strategies)
                {
                    MeasureRepeat(item.Key, item.Value);
                }

                Console.WriteLine("Done");
            }
        }

        #region Warmup

        private static void Warmup()
        {
            Console.WriteLine("Warm-up");

            CreateTree(10);

            var strategies = new List<KeyValuePair<string, IStrategy>>();
            var info = new KeyValuePair<string, IStrategy>(string.Empty, new SynchronicStrategy());
            strategies.Add(info);
            info = new KeyValuePair<string, IStrategy>(string.Empty, new BeginInvokeStrategy());
            strategies.Add(info);
            info = new KeyValuePair<string, IStrategy>(string.Empty, new ParallelTaskStrategy());
            strategies.Add(info);

            foreach (KeyValuePair<string, IStrategy> item in strategies)
            {
                Measure(item.Key, item.Value);
            }
        }

        #endregion // Warmup

        #region MeasureRepeat

        private static void MeasureRepeat(string title, IStrategy strategy)
        {
            for (int i = 0; i < REPEATS; i++)
            {
                Measure(title, strategy);
            }

            if (!string.IsNullOrEmpty(title))
            {
                Thread.Sleep(STRATEGY_SWITCH_SLEEP);
                Console.WriteLine();
            }
        }

        #endregion // MeasureRepeat

        #region Measure

        private static void Measure(string title, IStrategy strategy)
        {
            GC.Collect();
            if (!string.IsNullOrEmpty(title))
                Thread.Sleep(REPEAT_SLEEP);
            var sw = Stopwatch.StartNew();

            int total = strategy.Sum(_root);

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (!string.IsNullOrEmpty(title))
            {
                Console.WriteLine("{0}: {1:N0} || {2}", title, sw.ElapsedMilliseconds, total);
            }
            Console.ResetColor();
        }

        #endregion // Measure

        #region CreateTree

        /// <summary>
        /// initialize the tree
        /// </summary>
        private static void CreateTree(int size)
        {
            _root = new Node(size / 2);
            var rnd = new Random(42);
            var numbers = Enumerable.Range(0, size).ToList();
            numbers.RemoveAt(_root.GetValue());
            do
            {
                int index = rnd.Next(numbers.Count);
                int value = numbers[index];
                _root.Add(value);
                numbers.RemoveAt(index);
            } while (numbers.Count != 0);
        }

        #endregion // CreateTree
    }
}
