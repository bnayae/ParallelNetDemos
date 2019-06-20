using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cancellation_Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();

            var tasks = Enumerable.Range(0, 100)
                                   .Select(i => Task.Run(() => ExecAsync(i, cts.Token), cts.Token))
                                   .ToArray();
            await Task.Delay(120);
            Console.WriteLine("No cancel");
            cts.Cancel();
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception)
            {
                Console.WriteLine("cancelled");
            }

            Console.ReadKey();
        }

        private static async Task<int> ExecAsync(int i, CancellationToken cancellationToken)
        {
            Console.Write($"{i},");
            Thread.Sleep(1000);
            await Task.Delay(2);
            if (cancellationToken.IsCancellationRequested)
                return -1;
            Console.Write($"#{i},");
            return i;
        }
    }
}
