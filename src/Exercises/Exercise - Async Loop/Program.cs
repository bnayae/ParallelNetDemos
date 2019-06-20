using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Exercise___Async_Loop
{
    class Program
    {
        private const string URL = "https://source.unsplash.com/120x120";
        private const string URL_WITH_TOPIC = "https://source.unsplash.com/1200x1200/?{1}/";

        static void Main(string[] args)
        {
            // execute 100 iterations (with cancellation after 200 ms) 

            // 1. sequential 
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(4000));
            // Task t = SequentialAsync(cts.Token);

            // 2. all in parallel
            // Task t = AllAsync(cts.Token);

            // 3. with batch = 3
            Task t = BatchAsync(cts.Token);

            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(10);
            }
        }

        private static async Task SequentialAsync(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"Start {i}");
                byte[] image = await DownloadAsync(cancellationToken);
                Console.WriteLine($"Downloaded {i}");
                await File.WriteAllBytesAsync($"image{i}.jpg", image, cancellationToken);
                Console.WriteLine($"Saved {i}");
            }
        }

        private static Task AllAsync(CancellationToken cancellationToken)
        {
            var tasks = Enumerable.Range(0, 100)
                            .Select(i => DownloadAndSave(i, cancellationToken));
            return Task.WhenAll(tasks);
        }

        private static Task BatchAsync(CancellationToken cancellationToken)
        {
            var options = new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = 3,
                        CancellationToken = cancellationToken
                    };
            var ab = new ActionBlock<int>(i => DownloadAndSave(i, cancellationToken)
                        , options);
            for (int i = 0; i < 100; i++)
            {
                ab.Post(i);
            }
            ab.Complete();
            return ab.Completion;
        }

        private static async Task DownloadAndSave(int i, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Start {i}");
            byte[] image = await DownloadAsync(cancellationToken);
            Console.WriteLine($"Downloaded {i}");
            await File.WriteAllBytesAsync($"image{i}.jpg", image, cancellationToken);
            Console.WriteLine($"Saved {i}");
        }

        private static async Task<byte[]> DownloadAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var http = new HttpClient())
            {
                return await http.GetByteArrayAsync(URL);
            }
        }
    }
}
