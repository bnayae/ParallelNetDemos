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

            // ApmWithCallback();
            TaskWithCallback();

            Console.ReadKey(true);
        }

        #region APM

        private static void ApmWithCallback()
        {
            Func<int> f = () => { Thread.Sleep(2000); return 42; };
            IAsyncResult ar = f.BeginInvoke(ApmCallback, null);
            while (!ar.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }
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
            Task<int> t = Task.Run(f);
            Task t1 = t.ContinueWith(TaskCallback);

            #region More

            //Task[] ts = new Task[10];
            //for (int i = 0; i < 10; i++)
            //{
            //    int local = i;
            //    ts[i] = t.ContinueWith(c => Console.Write($"# {i} #,"));
            //}
            //Task.Factory.ContinueWhenAll(ts, all => Console.WriteLine("All 10 Done"));

            #endregion // More

            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }
        }

        private static void TaskCallback(Task<int> completedTask)
        {
            int result = completedTask.Result;
            Console.WriteLine(result);
            //Thread.Sleep(1000);
            //return "hello";
        }

        #endregion // Task
    }
}
