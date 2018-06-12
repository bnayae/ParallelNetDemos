using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sela.Samples
{
    public class Simple
    {
        public async Task ExecuteScenatio1Async()
        {
            var scheduler = new CustomScheduler();

            WriteInfo("1");
            await Task.Delay(10);
            WriteInfo("2");
            await Task.Factory.StartNew(async () =>
            {
                WriteInfo("3");
                await Task.Delay(10);
                WriteInfo("4");
            }, CancellationToken.None, TaskCreationOptions.None,
                   scheduler).Unwrap();
        }
        public async Task ExecuteScenatio2Async()
        {
            var scheduler = new CustomScheduler();

            WriteInfo("1");
            await Task.Factory.StartNew(async () =>
            {
                WriteInfo("2");
                await Task.Delay(10);
                WriteInfo("3");
            }, CancellationToken.None, TaskCreationOptions.None,
                   scheduler).Unwrap();
        }
        public async Task ExecuteScenatio3Async()
        {
            var scheduler = new CustomScheduler();

            WriteInfo("1");
            await Task.Factory.StartNew(async () =>
            {
                WriteInfo("2");
                await Task.Delay(10)
                    .ConfigureAwait(false);
                WriteInfo("3");
            }, CancellationToken.None, TaskCreationOptions.None,
                   scheduler).Unwrap();
        }

        private void WriteInfo(string title)
        {
            string cur = TaskScheduler.Current is CustomScheduler ? "Custom" : "ThreadPool";

            Console.WriteLine("{0}) Scheduler (Current = {1}), Pool = {2}, Id = {3}",
                title,
                cur,
                Thread.CurrentThread.IsThreadPoolThread,
                Thread.CurrentThread.ManagedThreadId);
        }
    }
}
