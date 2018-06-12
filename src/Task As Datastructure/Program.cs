using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task_As_Datastructure
{
    class Program
    {
        private readonly static ConcurrentQueue<AsyncToken> _buffer1 = new ConcurrentQueue<AsyncToken>();
        private readonly static BlockingCollection<AsyncToken> _buffer2 = new BlockingCollection<AsyncToken>();
        private static Timer _stage1Scheduler;
        static void Main(string[] args)
        {
            Write("Main");
            _stage1Scheduler = new Timer(HandleStage2Listener, "Global", 2000, 1000);
            Thread trd = new Thread(HandleStage3);
            trd.Start();

            Task<string> task1 = Stage1Async(1);
            Thread.Sleep(3000);
            Task<string> task2 = Stage1Async(2);

            Console.WriteLine($@"
================================================
Task 1 completed with value = {task1.Result}
Task 2 completed with value = {task2.Result}");

            Console.ReadKey();
        }

        private static Task<string> Stage1Async(int i)
        {
            Write("\r\n  Stage1");
            var asyncToken = new AsyncToken(i.ToString());
            _buffer1.Enqueue(asyncToken);
            return asyncToken.Completion;
        }

        private static void HandleStage2Listener(object state)
        {
            AsyncToken token;
            while (_buffer1.TryDequeue(out token))
            {
                Write($"\r\n    Stage2 Handling [{token}]");
                string value = $"#{token} by {state}";
                var nextToken = new AsyncToken(value, token);
                _buffer2.Add(nextToken);
            }
        }
        private static void HandleStage3()
        {
            foreach (AsyncToken token in _buffer2.GetConsumingEnumerable())
            {
                Write($"\r\n      Stage3 Handling [{token}]");
                Thread.Sleep(500);
                token.TryComplete();
            }
        }

        private static void Write(object title)
        {
            var trd = Thread.CurrentThread;
            Console.WriteLine($"{title} [{trd.ManagedThreadId}]: Pool = {trd.IsThreadPoolThread}");
        }
    }
}
