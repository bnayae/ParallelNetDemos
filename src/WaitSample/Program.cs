using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sela.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");

            Console.WriteLine("-------- Wait semantic ---------");
            SemanticWait();

            Console.WriteLine("\r\n=================== press any key to continue =======================");
            Console.ReadKey();

            Console.WriteLine("\r\n-------- Wait All -----------");
            WaitAllAny(true);

            Console.WriteLine("\r\n=================== press any key to continue =======================");
            Console.ReadKey();

            Console.WriteLine("\r\n-------- Wait Any -----------");
            WaitAllAny(false);

            Console.ReadKey();
        }

        #region SemanticWait

        private static void SemanticWait()
        {
            Task t1 = new Task(() =>
            {
                Console.WriteLine("Task 1 before sleep");
                Thread.Sleep(3000);
                Console.WriteLine("Task 1 after sleep");
            });

            Task t2 = new Task(() =>
            {
                Console.WriteLine("Task 2 before wait");
                t1.Wait();
                Console.WriteLine("Task 2 after wait");
            });

            t2.Start(TaskScheduler.Default);
            t1.Start(TaskScheduler.Default);

            t2.Wait();
        }

        #endregion // SemanticWait

        #region WaitAllAny

        private static void WaitAllAny(bool all)
        {
            var t1 = Task.Factory.StartNew(Exec, "#");
            Thread.Sleep(1000);
            var t2 = Task.Factory.StartNew(Exec, "*");

            if(all)
                Task.WaitAll(t1, t2);
            else
                Task.WaitAny(t1, t2);
            Console.WriteLine("\r\nBack to business");
        }

        #endregion // WaitAllAny

        #region Exec

        public static void Exec(object state)
        {
            var curThread = Thread.CurrentThread;
            var sw = Stopwatch.StartNew();
            long hang = TimeSpan.FromMilliseconds(10).Ticks;
            while (sw.ElapsedMilliseconds < 2000)
            {
                Console.Write(state);
                while (sw.ElapsedTicks % hang != 0) ;
            }
        }

        #endregion // Exec
    }    
}
