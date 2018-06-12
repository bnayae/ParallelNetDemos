using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
//using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

namespace ApmVsTask
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");

            //ApmWithCallback();
            TaskWithCallback();

            Console.ReadKey(true);
        }

        #region APM

        private static void ApmWithCallback()
        {
            Func<int> f = () => { Thread.Sleep(2000); return 42; };
            f.BeginInvoke(ApmCallback, null);
        }

        private static void ApmCallback(IAsyncResult ar)
        {
            #region How can you get the result?

            var tmp = ar as AsyncResult; // should we know it?
            Func<int> f = (tmp.AsyncDelegate as Func<int>); // should we know it?
            int result = f.EndInvoke(ar);
            Console.WriteLine(result);

            #endregion // How can you get the result?
        }

        #endregion // APM

        #region Task

        private static void TaskWithCallback()
        {
            Func<int> f = () => { Thread.Sleep(2000); return 42; };
            Task<int> t = Task.Factory.StartNew(f);
            Task<string> t1 = t.ContinueWith<string>(TaskCallback);
            t1.ContinueWith(c => Console.WriteLine("Callback completed: {0}", c.Result));

            Task[] tasks = new Task[10];
            for (int i = 0; i < 10; i++)
            {
                int local = i;
                tasks[i] = t.ContinueWith(c =>
                    {
                        Thread.Sleep(500 * local);
                        Console.Write("{0},", local);
                    });
            }
            Task.Factory.ContinueWhenAll(tasks, ts => Console.WriteLine("All completed"));

            IEnumerable<Task<int>> data = from item in Enumerable.Range(0, 10)
                       select Task.Factory.StartNew(i =>
                       {
                           Console.Write(".");
                           var value = (int)i;
                           Thread.Sleep(500 * value);
                           Console.Write("X");
                           return value;
                       }, item);

            //int sum = data.Sum(tx => tx.Result);
            Task.Factory.ContinueWhenAll(data.ToArray(), c =>
                {
                    int sum = c.Sum(tx => t.Result);
                    Console.WriteLine("Sum {0}", sum);
                });
            Console.WriteLine("DONE");
        }

        private static string TaskCallback(Task<int> completedTask)
        {
            int result = completedTask.Result;
            Console.WriteLine(result);
            Thread.Sleep(1000);
            return "hello";
        }

        #endregion // Task
    }
}
