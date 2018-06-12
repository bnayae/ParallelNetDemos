using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tpl.Samples
{
    class Program
    {
        private static readonly object _sync1 = new object();
        private static readonly object _sync2 = new object();

        static void Main(string[] args)
        {
            var t1 = Task.Factory.StartNew (state =>
            {
                //lock (_sync1)
                using (var l1 = LeveledLock.Acquire(_sync1))
                {
                    Console.WriteLine("L1: Level={0}", l1.Level);
                    Thread.Sleep(100);
                    //lock (_sync2)
                    using (var l2 = LeveledLock.Acquire(_sync2))
                    {
                        Console.WriteLine("L2: Level={0}", l2.Level);
                        Console.WriteLine("Done 1");
                    }
                }
            }, "T1");

            //t1.Wait();

            var t2 = Task.Factory.StartNew(state =>
            {
                //lock (_sync2)
                using (var l2 = LeveledLock.Acquire(_sync2))
                {
                    Console.WriteLine("L2: Level={0}", l2.Level);
                    Thread.Sleep(100);
                    //lock (_sync1)
                    using (var l1 = LeveledLock.Acquire(_sync1))
                    {
                        Console.WriteLine("L1: Level={0}", l1.Level);
                        Console.WriteLine("Done 1");
                    }
                }
            }, "T2");

            // using continuation to write errors
            t1.ContinueWith(t => Console.WriteLine("{0}: {1}", t.AsyncState, t.Exception.InnerExceptions[0].Message),
                TaskContinuationOptions.OnlyOnFaulted);
            t2.ContinueWith(t => Console.WriteLine("{0}: {1}", t.AsyncState, t.Exception.InnerExceptions[0].Message),
                TaskContinuationOptions.OnlyOnFaulted);

            // using continuation to write complete
            t1.ContinueWith(t => Console.WriteLine("{0} Complete", t.AsyncState),
                TaskContinuationOptions.NotOnFaulted);
            t2.ContinueWith(t => Console.WriteLine("{0} Complete", t.AsyncState),
                TaskContinuationOptions.NotOnFaulted);

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
