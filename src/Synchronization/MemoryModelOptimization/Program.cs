using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MemoryModelOptimization
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isComplete = false;
            var sync = new ManualResetEventSlim(false);
            var t = ThreadPool.QueueUserWorkItem(state =>
            {
                bool toggle = false;
                while (!isComplete)
                {
                    toggle = !toggle;
                }
                sync.Set();
            });
            Thread.Sleep(1000);
            isComplete = true;
            sync.Wait();       // Blocks to indefinitely and beyond
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
