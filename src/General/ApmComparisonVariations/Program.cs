using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;

namespace Tpl.Samples
{
    class Program
    {
        #region Constants

        private const int STRATEGY_SWITCH_SLEEP = 1000;
        private const int REPEAT_SLEEP = 500;
        private const int REPEATS = 2;
        private static readonly int RANGE_SIZE = 50000 * Environment.ProcessorCount;
        private const int SUM_SIZE = 2048;

        #endregion // Constants

        private const int EVEN_WAIT_DURATION = 5;
        private static int _rangeSize = 1;
        private static int[] _range; // when the number can be cached at the core level it make a big different whether the thread jump over cores 
        private static Action<object> _currentAction = (o) => { };

        static void Main(string[] args)
        {
            Warmup();

            bool supportThread = true;

            #region Console.WriteLine("Choose action:")

            Console.WriteLine("Choose action:");
            Console.WriteLine("e. even wait");
            Console.WriteLine("u. uneven wait");
            Console.WriteLine("s. Sqrt");
            Console.WriteLine("p. Prime number");
            Console.WriteLine("c. Cache effect");
            Console.WriteLine("q. quit");

            #endregion // Console.WriteLine("Choose action:")

            char selection = Console.ReadKey().KeyChar;

            #region switch (selection)

            switch (selection)
            {
                case 'e':
                    _currentAction = EvenWait;
                    _rangeSize = 100 * Environment.ProcessorCount;
                    break;
                case 'u':
                    _currentAction = UnevenWait;
                    _rangeSize = 50 * Environment.ProcessorCount;
                    break;
                case 's':
                    _currentAction = Sqrt;
                    _rangeSize = 3000000 * Environment.ProcessorCount;
                    supportThread = false;
                    break;
                case 'p':
                    _currentAction = PrimeNumber;
                    _rangeSize = 500000 * Environment.ProcessorCount;
                    _range = Enumerable.Range(2, (int)Math.Sqrt(_rangeSize)).ToArray();
                    break;
                case 'c':
                    _currentAction = Sum;
                    _rangeSize = 200000 * Environment.ProcessorCount;
                    _range = Enumerable.Range(0, _rangeSize).ToArray();
                    supportThread = false;
                    break;
                default:
                    return;
            }

            #endregion // switch (selection)

            #region ConsoleWrite (...)

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nDegree of parallelism = {0:N0}", _rangeSize);
            Console.WriteLine();
            Console.ResetColor();

            #endregion // ConsoleWrite (...)

            while (true)
            {
                #region Console.WriteLine("Choose strategy:")

                Console.WriteLine("Choose strategy:");
                Console.WriteLine("0. Synchronic");
                Console.WriteLine("1. Parallel For");
                Console.WriteLine("2. Parallel Task");
                Console.WriteLine("3. Thread Pool");
                Console.WriteLine("4. Thread");
                if (supportThread)
                    Console.WriteLine("8. All");
                Console.WriteLine("9. All but Thread");
                Console.WriteLine("q. quit");

                #endregion // Console.WriteLine("Choose strategy:")

                selection = Console.ReadKey().KeyChar;
                Console.WriteLine(Environment.NewLine);
                var strategies = new List<KeyValuePair<string, IStrategy>>();

                #region switch (selection)

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
                            var info = new KeyValuePair<string, IStrategy>("Parallel For:", new ParallelForStrategy());
                            strategies.Add(info);
                            break;
                        }
                    case '2':
                        {
                            var info = new KeyValuePair<string, IStrategy>("Parallel Task:", new ParallelTaskStrategy());
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
                            var info = new KeyValuePair<string, IStrategy>("Thread:", new ThreadStrategy());
                            strategies.Add(info);
                            break;
                        }
                    case '8':
                        {
                            var info = new KeyValuePair<string, IStrategy>("Synchronic:", new SynchronicStrategy());
                            strategies.Add(info);
                            info = new KeyValuePair<string, IStrategy>("Parallel For:", new ParallelForStrategy());
                            strategies.Add(info);
                            info = new KeyValuePair<string, IStrategy>("Parallel Task:", new ParallelTaskStrategy());
                            strategies.Add(info);
                            info = new KeyValuePair<string, IStrategy>("Thread Pool:", new ThreadPoolStrategy());
                            strategies.Add(info);
                            info = new KeyValuePair<string, IStrategy>("Thread:", new ThreadStrategy());
                            strategies.Add(info);
                            break;
                        }
                    case '9':
                        {
                            var info = new KeyValuePair<string, IStrategy>("Synchronic:", new SynchronicStrategy());
                            strategies.Add(info);
                            info = new KeyValuePair<string, IStrategy>("Parallel For:", new ParallelForStrategy());
                            strategies.Add(info);
                            info = new KeyValuePair<string, IStrategy>("Parallel Task:", new ParallelTaskStrategy());
                            strategies.Add(info);
                            info = new KeyValuePair<string, IStrategy>("Thread Pool:", new ThreadPoolStrategy());
                            strategies.Add(info);
                            break;
                        }
                    default:
                        return;
                }

                #endregion // switch (selection)

                foreach (KeyValuePair<string, IStrategy> item in strategies)
                {
                    MeasureRepeat(item.Key, item.Value);
                    item.Value.Dispose();
                }
                Console.WriteLine();
            }
        }

        #region Actions

        #region EvenWait

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void EvenWait(object state)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < EVEN_WAIT_DURATION * 1.5) ; // spin for the duration
            sw.Stop();
        }

        #endregion // EvenWait

        #region UnevenWait

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void UnevenWait(object state)
        {
            int i = (int)state;
            int factor = (_rangeSize / 2) > i ? 5 : 1; // create uneven wait for upper range

            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < EVEN_WAIT_DURATION * factor) ; // spin for the duration
            sw.Stop();
        }

        #endregion // UnevenWait

        #region SpinEvenWait

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SpinEvenWait(object state)
        {
            SpinWait.SpinUntil(() => false, EVEN_WAIT_DURATION);
        }

        #endregion // SpinEvenWait

        #region SpinUnevenWait

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SpinUnevenWait(object state)
        {
            int duration = (int)state;
            SpinWait.SpinUntil(() => false, duration);
        }

        #endregion // SpinUnevenWait

        #region Sqrt

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void Sqrt(object state)
        {
            int i = (int)state;
            Math.Sqrt(i);
        }

        #endregion // Sqrt

        #region PrimeNumber

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void PrimeNumber(object state)
        {
            int number = (int)state;
            // IT SHOW THE CACHE EFFECT BUT THE UNBALANCE PARTITION DOES NOT HAS ANY REAL EFFECT (BECAUSE THE WORK STILLING)
            // this sample show the effect of unbalance processing time
            // that way we do not optimized it with !range.Any(i => number % i == 0)
            var isPrime = _range.Take((int)Math.Sqrt(number)).All(i => number % i > 0);
        }

        #endregion // PrimeNumber

        #region Sum

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void Sum(object state)
        {
            const int ARR_SIZE = 1024;
            int number = (int)state;
            // this sample show the effect of cache usage on Range partitioning in compare with Chunk partitioning
            var arr = new int[ARR_SIZE];
            int left = _range.Length - number;
            int copyAmount = left < ARR_SIZE ? left : ARR_SIZE;
            Array.Copy(_range, number, arr, 0, copyAmount);
            var result = arr.Sum();
        }

        #endregion // Sum

        #endregion // Actions

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

            strategy.Execute(_rangeSize, _currentAction);

            sw.Stop();
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (!string.IsNullOrEmpty(title))
            {
                Console.WriteLine("{0}: {1:N0}", title, sw.ElapsedMilliseconds);
            }
            Console.ResetColor();
        }

        #endregion // Measure

        #region Warmup

        private static void Warmup()
        {
            Console.WriteLine("Warm-up");

            var strategies = new List<KeyValuePair<string, IStrategy>>();
            var info = new KeyValuePair<string, IStrategy>(string.Empty, new SynchronicStrategy());
            strategies.Add(info);
            info = new KeyValuePair<string, IStrategy>(string.Empty, new ParallelForStrategy());
            strategies.Add(info);
            info = new KeyValuePair<string, IStrategy>(string.Empty, new ParallelTaskStrategy());
            strategies.Add(info);
            info = new KeyValuePair<string, IStrategy>(string.Empty, new ThreadPoolStrategy());
            strategies.Add(info);
            info = new KeyValuePair<string, IStrategy>(string.Empty, new ThreadStrategy());
            strategies.Add(info);

            foreach (KeyValuePair<string, IStrategy> item in strategies)
            {
                Measure(item.Key, item.Value);
                item.Value.Dispose();
            }
        }

        #endregion // Warmup
    }
}
