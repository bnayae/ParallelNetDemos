using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AwaitAndScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread t = Thread.CurrentThread;
            Console.WriteLine("Main Thread Name = {0}, Id = {1}",
                t.Name,
                t.ManagedThreadId);

            var scd = new PoolScheduler(3);

            Exec(scd);
            //Exec(TaskScheduler.Default);

            //for (int i = 0; i < 10; i++)
            //{
            //    Exec(scd);
            //    //Exec(TaskScheduler.Default);
            //}

            Console.ReadKey();
        }

        private static Task Exec(TaskScheduler scd)
        {
            // Start Scheduled Task
            return Task.Factory.StartNew(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    lock (scd)
                    {
                        #region Console.ForegroundColor = ...

                        if (Console.ForegroundColor == ConsoleColor.Yellow)
                            Console.ForegroundColor = ConsoleColor.White;
                        else
                            Console.ForegroundColor = ConsoleColor.Yellow;

                        #endregion // Console.ForegroundColor = ...

                        Thread t = Thread.CurrentThread;
                        Console.WriteLine("{0}: Thread Id = {1},     Task ID = {2}",
                            t.Name,
                            t.ManagedThreadId,
                            Task.CurrentId);
                    }

                    await Task.Delay(1000);
                };
            }, CancellationToken.None, TaskCreationOptions.None, scd);
        }
    }
}
