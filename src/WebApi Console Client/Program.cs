using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi_Console_Client
{
    class Program
    {
        private const string URL = "http://localhost:5287/api/simple/";
        private const int THROUGHPUT = 80;
        private const string SYNC_URL = URL + "sync";
        private const string BLOCKING_RUN_URL = URL + "run-blocking";
        private const string ASYNC_RUN_URL = URL + "run-async";
        private const string ASYNC_URL = URL + "async";
        static void Main(string[] args)
        {
            //ThreadPool.SetMaxThreads(Environment.ProcessorCount, Environment.ProcessorCount);
            Task t = BanchmarkAsync();
            t.Wait();
        }

        private static async Task BanchmarkAsync()
        {
            // warm up
            Console.WriteLine("Warm-up");
            await Task.WhenAll(
                ExecAsync(SYNC_URL, 6),
                ExecAsync(BLOCKING_RUN_URL, 6),
                ExecAsync(ASYNC_RUN_URL, 6),
                ExecAsync(ASYNC_URL, 6));
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            Console.Clear();

            while (true)
            {
                Console.WriteLine("Sync (sleep)");
                await ExecAsync(SYNC_URL, THROUGHPUT);
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                Console.Clear();

                Console.WriteLine("Task.Run (sleep)");
                await ExecAsync(BLOCKING_RUN_URL, THROUGHPUT);
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                Console.Clear();

                Console.WriteLine("Task.Run (delay)");
                await ExecAsync(ASYNC_RUN_URL, THROUGHPUT);
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                Console.Clear();

                Console.WriteLine("Async (delay)");
                await ExecAsync(ASYNC_URL, THROUGHPUT);
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                Console.Clear();
            }
        }
        private static async Task ExecAsync(string url, int throughput)
        {
            using (var http = new HttpClient())
            {
                var sw = Stopwatch.StartNew();
                var tasks = from i in Enumerable.Range(1, throughput)
                            select GetAsync(http, url);
                await Task.WhenAll(tasks);
                sw.Stop();
                Console.WriteLine($"Duration = {sw.Elapsed}");
            }
        }

        private static async Task GetAsync(HttpClient http, string url)
        {
            string data = await http.GetStringAsync(url);
            Console.Write($"{data},");
        }
    }
}
