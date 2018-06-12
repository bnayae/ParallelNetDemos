#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics;

#endregion // Using

    /// the sample show the benefits of hierarchical cancelation

namespace Tpl.Samples
{
    class Program
    {
        #region Constants

        public const int SPIN_MILLISECONDS = 10;
        private const int STRATEGY_SWITCH_SLEEP = 400;
        private const int REPEAT_SLEEP = 200;
        private const int REPEATS = 2; 
        private const int RANGE_SIZE = 100;

        #endregion // Constants

        private static int _valueToFind;
        private static Node _root;
        private static int _treeSize = 5;

        static void Main(string[] args)
        {
            Warmup();
            _treeSize = RANGE_SIZE;
            CreateTree();

            while (true)
            {
                Console.WriteLine("Size = {0}", RANGE_SIZE);
                var strategies = new List<KeyValuePair<string, IStrategy>>();

                #region Console.WriteLine("Choose strategy:")

                Console.WriteLine("Choose strategy:");
                Console.WriteLine("0. Synchronic");
                Console.WriteLine("1. Parallel Task");
                Console.WriteLine("2. Parallel Task with cancellation");
                Console.WriteLine("3. Thread Pool");
                Console.WriteLine("3. Thread Pool with cancellation");
                Console.WriteLine("9. All");
                Console.WriteLine("q. quit");

                var selection = Console.ReadKey().KeyChar;
                Console.WriteLine(Environment.NewLine);
                switch (selection)
                {
                    case '0':
                        {
                            var info = new KeyValuePair<string, IStrategy>("Synchronic:", new SynchronicStrategy());
                            strategies.Add(info);
                            break;
                        }
                    case '1':
                        {
                            var info = new KeyValuePair<string, IStrategy>("Parallel Task:", new ParallelTaskStrategy());
                            strategies.Add(info);
                            break;
                        }
                    case '2':
                        {
                            var info = new KeyValuePair<string, IStrategy>("Parallel Task with cancellation:", new ParallelTaskWithCancellationStrategy());
                            strategies.Add(info);
                            break;
                        }
                    case '3':
                        {
                            var info = new KeyValuePair<string, IStrategy>("Thread Pool:", new ThreadPoolStrategy());
                            strategies.Add(info);
                            break;
                        }
                    case '4':
                        {
                            var info = new KeyValuePair<string, IStrategy>("Thread Pool with cancellation:", new ThreadPoolWithCancellationStrategy());
                            strategies.Add(info);
                            break;
                        }
                    case '9':
                        {
                            var info = new KeyValuePair<string, IStrategy>("Synchronic:", new SynchronicStrategy());
                            strategies.Add(info);
                            info = new KeyValuePair<string, IStrategy>("Parallel Task:", new ParallelTaskStrategy());
                            strategies.Add(info);
                            info = new KeyValuePair<string, IStrategy>("Parallel Task with cancellation:", new ParallelTaskWithCancellationStrategy());
                            strategies.Add(info);
                            info = new KeyValuePair<string, IStrategy>("Thread Pool:", new ThreadPoolStrategy());
                            strategies.Add(info);
                            info = new KeyValuePair<string, IStrategy>("Thread Pool with cancellation:", new ThreadPoolWithCancellationStrategy());
                            strategies.Add(info);
                            break;
                        }
                    default:
                        return;
                }

                #endregion // Console.WriteLine("Choose strategy:")

                foreach (KeyValuePair<string, IStrategy> item in strategies)
                {
                    WriteWithColor("Find near", ConsoleColor.Green);
                    _valueToFind = _root.ChildNodes[1].Value; // value neer the root

                    MeasureRepeat(item.Key, item.Value);

                    WriteWithColor("Find far", ConsoleColor.Green);
                    Node n = _root;
                    while (n.ChildNodes.Any())
                        n = n.ChildNodes.Last();
                    _valueToFind = n.Value;

                    MeasureRepeat(item.Key, item.Value);

                    WriteWithColor("Find none", ConsoleColor.Green);
                    _valueToFind = -2;

                    MeasureRepeat(item.Key, item.Value);
                    _valueToFind = n.Value;

                    item.Value.Dispose();
                }
            }
        }

        #region Warmup

        private static void Warmup()
        {
            Console.WriteLine("Warm-up");

            CreateTree();

            var strategies = new List<KeyValuePair<string, IStrategy>>();
            var info = new KeyValuePair<string, IStrategy>(string.Empty, new SynchronicStrategy());
            strategies.Add(info);
            info = new KeyValuePair<string, IStrategy>(string.Empty, new ParallelTaskStrategy());
            strategies.Add(info);
            info = new KeyValuePair<string, IStrategy>(string.Empty, new ParallelTaskWithCancellationStrategy());
            strategies.Add(info);
            info = new KeyValuePair<string, IStrategy>(string.Empty, new ThreadPoolStrategy());
            strategies.Add(info);
            info = new KeyValuePair<string, IStrategy>(string.Empty, new ThreadPoolWithCancellationStrategy());
            strategies.Add(info);

            foreach (KeyValuePair<string, IStrategy> item in strategies)
            {
                Measure(item.Key, item.Value);
            }

            Console.WriteLine();
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
            var sync = new CountdownEvent(_treeSize);

            strategy.Reset();
            strategy.Find(_root, _valueToFind, () =>
                {
                    Console.WriteLine("Found");
                    strategy.Cancel(); // cancel the operation
                }, sync);
            try
            {
                sync.Wait(strategy.Token); // stop waiting on cancellation
            }
            #region Exception Handling

            catch (OperationCanceledException)
            {
                Debug.WriteLine("Canceled");
            }

            #endregion // Exception Handling
            sw.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;
            if (!string.IsNullOrEmpty(title))
            {
                Console.WriteLine("{0}: {1:N0}", title, sw.ElapsedMilliseconds);
            }
            Console.ResetColor();
        }

        #endregion // Measure

        #region CreateTree

        /// <summary>
        /// initialize the tree
        /// </summary>
        private static void CreateTree()
        {
            _root = new Node(_treeSize / 2);
            var rnd = new Random(42);
            var numbers = Enumerable.Range(0, _treeSize).ToList();
            numbers.RemoveAt(_root.Value);
            do
            {
                int index = rnd.Next(numbers.Count);
                int value = numbers[index];
                _root.Add(value);
                numbers.RemoveAt(index);
            } while (numbers.Count != 0);
        }

        #endregion // CreateTree

        #region WriteWithColor

        private static void WriteWithColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        #endregion // WriteWithColor
    }
}
