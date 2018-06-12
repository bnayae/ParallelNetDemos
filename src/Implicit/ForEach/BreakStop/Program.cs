using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace BreakStop
{
    class Program
    {
        [DllImport("Kernel32.dll"), SuppressUnmanagedCodeSecurity]
        private static extern int GetCurrentProcessorNumber();

        private static ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        private const int STOP_AT = 11;
        //private const int BREAK_AT = 50;
        private const int BREAK_AT = 33;

        static void Main(string[] args)
        {
            ParallelLoopResult loopResult =
                Parallel.For(0, 100, (int i, ParallelLoopState loop) =>
                {
                    if (i == STOP_AT && loop.LowestBreakIteration == null)
                    {
                        loop.Stop(); // race condition is possible, do not mix Stop and Break
                        _queue.Enqueue("Stop");
                    }
                    else if (i == BREAK_AT && !loop.ShouldExitCurrentIteration)
                    {
                        loop.Break();
                        _queue.Enqueue("Break");
                    }

                    int core = GetCurrentProcessorNumber();
                    string data = string.Format("{0}{1},", new string(' ', core * 2), i);
                    _queue.Enqueue(data);
                });

            if (loopResult.IsCompleted)
                Console.WriteLine("Complete");
            else if (loopResult.LowestBreakIteration == null)
                Console.WriteLine("Stopped");
            else
                Console.WriteLine("Break at " + loopResult.LowestBreakIteration);

            foreach (var item in _queue)
            {
                Console.WriteLine(item);
            }
        }
    }
}
