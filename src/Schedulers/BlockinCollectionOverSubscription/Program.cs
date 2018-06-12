#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

#endregion // Using

// the issue was found by Guy Eden from ITG

namespace Bnaya.Samples
{
    class Program
    {
        private static Timer _tmr;

        [MTAThread]
        private static void Main()
        {
            var bc = new BlockingCollection<int>();

            Task.Factory.StartNew(() => 
                {
                    ParallelOptions options = new ParallelOptions
                    {
                        TaskScheduler = new LongRunningTaskScheduler(30),
                        //MaxDegreeOfParallelism = 30
                    };
                    Parallel.ForEach(bc.GetConsumingEnumerable(), options, i => { Console.WriteLine("."); });
                });
            _tmr = new Timer(state => bc.Add(1), null,  5000, 1000);
            Thread.Sleep(-1);
        }
    }
}
