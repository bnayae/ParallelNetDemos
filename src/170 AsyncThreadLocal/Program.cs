using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _170_AsyncThreadLocal
{
    public class Program
    {
        private const bool _useScheduler = true;
        private static ThreadLocal<NonThreadSafeClient> _threadLocal = new ThreadLocal<NonThreadSafeClient>();
        private static AsyncLocal<NonThreadSafeClient> _asyncLocal = new AsyncLocal<NonThreadSafeClient>();

        public static void Main()
        {
            ThreadPool.SetMinThreads(10, 2);
            var t = A();
            t.Wait();
            Console.ReadKey();
        }

        static async Task A()
        {
            await B();

            //await Task.Delay(100);
            await Delay();
            await B();
            Info("A");
        }

        static async Task B()
        {
            using (var client = new NonThreadSafeClient())
            {
                CallContext.LogicalSetData("Client", client);
                _asyncLocal.Value = client;
                _threadLocal.Value = client;
                Info("Enter B");

                await C();

                await Delay();

                await C();
                Info("Exit B");
            }
        }

        static async Task C()
        {
            await Delay();

            Info("C");
        }

        private static Task Delay()
        {
            if (_useScheduler)
                return Task.Factory.StartNew(() => Thread.Sleep(100), CancellationToken.None, TaskCreationOptions.None, new Scd());
            else
                return Task.Delay(100);
        }
        private static void Info(string title)
        {
            var threadContext = _threadLocal.Value;
            var asyncContext = _asyncLocal.Value;
            var callContext = CallContext.LogicalGetData("Client") as NonThreadSafeClient;

            var tId = Thread.CurrentThread.ManagedThreadId;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{title} (Async):   {asyncContext?.Id}, Thread Id = {tId}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{title} (Context): {callContext?.Id}, Thread Id = {tId}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{title} (Thread):  {threadContext?.Id}, Thread Id = {tId}");
            Console.WriteLine("Press any key");
            Console.WriteLine();
            Console.ReadKey(true);

        }

        private class Scd : TaskScheduler
        {

            protected override IEnumerable<Task> GetScheduledTasks()
            {
                return Enumerable.Empty<Task>();
            }

            protected override void QueueTask(Task task)
            {
                Thread t = new Thread(() => base.TryExecuteTask(task));
                t.Start();
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                return false;
            }
        }
    }




}
