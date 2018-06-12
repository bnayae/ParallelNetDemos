using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadHistory
{
    class Program
    {
        private const int MAX = 100_000_000;
        private const int TRD_COUNT = 10;
        private static readonly object _lck = new object();

        static void Main(string[] args)
        {
            Console.WriteLine($"Server GC = {GCSettings.IsServerGC}");

            Synchronic();
            Unsafe();
            //Locking();
            //Interlocking();
            DataLocality();

            Console.WriteLine();
            Console.WriteLine("=============================================================");
            Console.WriteLine();

            // Show the rest after learning the Implicit and Explicit parallelism (latter chapters)

            //ParallelFor();
            //ThreadLocal();
            //ForEachPartitioner();
            //StructuralFor();
        }

        #region Synchronic

        private static void Synchronic()
        {
            int index = 0;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < MAX; i++)
            {
                index++;
            }
            sw.Stop();
            Console.WriteLine("Synchronic: _index = {0}, Duration = {1}", index, sw.ElapsedMilliseconds);
            Console.WriteLine("------------------------------------------------------------");
        }

        #endregion // Synchronic

        #region Unsafe

        private static void Unsafe()
        {
            int index = 0;
            var sw = Stopwatch.StartNew();
            using (CountdownEvent sync = new CountdownEvent(TRD_COUNT))
            {
                for (int j = 0; j < TRD_COUNT; j++)
                {
                    ThreadPool.QueueUserWorkItem(state =>
                        {
                            for (int i = 0; i < MAX / TRD_COUNT; i++)
                            {
                                index++;
                            }
                            sync.Signal();
                        }, null);
                }
                sync.Wait(); // Do use a overload with timeout
            }
            sw.Stop();
            Console.WriteLine("Unsafe: _index = {0}, Duration = {1}", index, sw.ElapsedMilliseconds);
            Console.WriteLine("------------------------------------------------------------");
        }

        #endregion // Unsafe

        #region Locking

        private static void Locking()
        {
            int index = 0;
            var sw = Stopwatch.StartNew();
            using (CountdownEvent sync = new CountdownEvent(TRD_COUNT))
            {
                for (int j = 0; j < TRD_COUNT; j++)
                {
                    ThreadPool.QueueUserWorkItem(state =>
                        {
                            for (int i = 0; i < MAX / TRD_COUNT; i++)
                            {
                                lock (_lck)
                                {
                                    index++;
                                }
                            }
                            sync.Signal();
                        }, null);
                }
                sync.Wait(); // Do use a overload with timeout
            }
            sw.Stop();
            Console.WriteLine("Locking: _index = {0}, Duration = {1}", index, sw.ElapsedMilliseconds);
            Console.WriteLine("------------------------------------------------------------");
        }

        #endregion // Locking

        #region Interlocking

        private static void Interlocking()
        {
            int index = 0;
            var sw = Stopwatch.StartNew();
            using (CountdownEvent sync = new CountdownEvent(TRD_COUNT))
            {
                for (int j = 0; j < TRD_COUNT; j++)
                {
                    ThreadPool.QueueUserWorkItem(state =>
                        {
                            for (int i = 0; i < MAX / TRD_COUNT; i++)
                            {
                                Interlocked.Increment(ref index);
                            }
                            sync.Signal();
                        }, null);
                }
                sync.Wait(); // Do use a overload with timeout
            }
            sw.Stop();
            Console.WriteLine("Interlocking: _index = {0}, Duration = {1}", index, sw.ElapsedMilliseconds);
            Console.WriteLine("------------------------------------------------------------");
        }

        #endregion // Interlocking

        #region DataLocality

        private static void DataLocality()
        {
            int index = 0;
            var sw = Stopwatch.StartNew();
            using (CountdownEvent sync = new CountdownEvent(TRD_COUNT))
            {
                for (int j = 0; j < TRD_COUNT; j++)
                {
                    ThreadPool.QueueUserWorkItem(state =>
                        {
                            int local = 0;
                            for (int i = 0; i < MAX / TRD_COUNT; i++)
                            {
                                local++;
                            }
                            Interlocked.Add(ref index, local);
                            sync.Signal();
                        }, null);
                }
                sync.Wait(); // Do use a overload with timeout
            }
            sw.Stop();
            Console.WriteLine("DataLocality: _index = {0}, Duration = {1}", index, sw.ElapsedMilliseconds);
            Console.WriteLine("------------------------------------------------------------");
        }

        #endregion // DataLocality

        #region ParallelFor

        private static void ParallelFor()
        {
            int index = 0;
            var sw = Stopwatch.StartNew();
            Parallel.For(0, MAX, i => Interlocked.Increment(ref index));
            sw.Stop();
            Console.WriteLine("ParallelFor: _index = {0}, Duration = {1}", index, sw.ElapsedMilliseconds);
            Console.WriteLine("------------------------------------------------------------");
        }

        #endregion // ParallelFor

        #region ThreadLocal

        private static void ThreadLocal()
        {
            int index = 0;
            var sw = Stopwatch.StartNew();
            using (ThreadLocal<int> local = new ThreadLocal<int>(() => 0, trackAllValues: true))
            {
                //Parallel.For(0, MAX, i => local.Value++);
                using (CountdownEvent sync = new CountdownEvent(TRD_COUNT))
                {
                    for (int j = 0; j < TRD_COUNT; j++)
                    {
                        ThreadPool.QueueUserWorkItem(state =>
                        {
                            for (int i = 0; i < MAX / TRD_COUNT; i++)
                            {
                                local.Value++;
                            }
                            sync.Signal();
                        }, null);
                    }
                    sync.Wait(); // Do use a overload with timeout
                }
                foreach (int item in local.Values)
                {
                    index += item;
                }
            }
            sw.Stop();
            Console.WriteLine("ThreadLocal: _index = {0}, Duration = {1}", index, sw.ElapsedMilliseconds);
            Console.WriteLine("------------------------------------------------------------");
        }

        #endregion // ThreadLocal

        #region ForEachPartitioner

        private static void ForEachPartitioner()
        {
            int index = 0;
            var sw = Stopwatch.StartNew();
            var partitioner = Partitioner.Create(0, MAX);
            Parallel.ForEach(partitioner, range =>
                {
                    int local = 0;
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        local++;
                    }
                    Interlocked.Add(ref index, local);
                });
            sw.Stop();
            Console.WriteLine("ForEachPartitioner: _index = {0}, Duration = {1}", index, sw.ElapsedMilliseconds);
            Console.WriteLine("------------------------------------------------------------");
        }

        #endregion // ForEachPartitioner

        #region StructuralFor

        private static void StructuralFor()
        {
            int index = 0;
            var sw = Stopwatch.StartNew();
            var partitioner = Partitioner.Create(0, MAX);
            Parallel.For(0, MAX, 
                () => 0, // init
                (int local, ParallelLoopState state, int i) => local + 1, // next value
                local => Interlocked.Add(ref index, local)); // aggregate
            sw.Stop();
            Console.WriteLine("StructuralFor: _index = {0}, Duration = {1}", index, sw.ElapsedMilliseconds);
            Console.WriteLine("------------------------------------------------------------");
        }

        #endregion // ForEachPartitioner
    }
}
