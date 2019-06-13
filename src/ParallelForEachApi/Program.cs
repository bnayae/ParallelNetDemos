using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelForEachApi
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            var items = Enumerable.Range(0, 20);
            var queue = new ConcurrentQueue<int>();
            Parallel.ForEach(items, async i =>
            {
                await Task.Delay(1000);
                queue.Enqueue(i);
                // Thread.Sleep(1000);
                Console.Write($"{i}, ");
            });
            Console.WriteLine($"Done: {string.Join(",", queue)}");
            Console.ReadKey();
        }
    }
}
