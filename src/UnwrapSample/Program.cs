using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnwrapSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");

            NestingTaskWithFactory();
            //NestingTaskWithRun();
            //CompletionTiming();

            Console.ReadKey();
        }

        #region NestingTaskWithFactory

        private static void NestingTaskWithFactory()
        {
            Task<Task<int>> t = Task.Factory.StartNew(() =>
                {
                    Task<int> nested = Task.Run(() => 42); // it is not a parent child semantic
                    return nested; // return Task<int> which will be wrapped by the outer task
                });

            Console.WriteLine("Processing");
            
            // int i = t.Result.Result;
            int i = t.Unwrap().Result;
            Console.WriteLine(i);
        }

        #endregion // NestingTaskWithFactory

        #region NestingTaskWithRun

        private static void NestingTaskWithRun()
        {
            Task<int> t = Task.Run(() => // Task.Run automatically unwrap nested task
                {
                    Task<int> nested = Task.Run(() => 42); // it is not a parent child semantic
                    return nested; // return Task<int> which will be wrapped by the outer task
                });

            Console.WriteLine("Processing");
            
            int i = t.Result;
            Console.WriteLine(i);
        }

        #endregion // NestingTaskWithRun

        #region NestingTaskContinuation

        private static void NestingTaskContinuation()
        {
            Task<Task<int>> t = Task.Delay(1000)
                .ContinueWith( completed => // Task.Run automatically unwrap nested task
                {
                    Task<int> nested = Task.Run(() => 42); // it is not a parent child semantic
                    return nested; // return Task<int> which will be wrapped by the outer task
                });

            Console.WriteLine("Processing");

            // int i = t.Result.Result;
            int i = t.Unwrap().Result;
            Console.WriteLine(i);
        }

        #endregion // NestingTaskContinuation

        #region CompletionTiming

        private static void CompletionTiming()
        {
            Task<Task<int>> t = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Begin Parent");

                Task<int> child = Task.Run(() =>
                {
                    Console.WriteLine("Begin Child");
                    Thread.Sleep(1000);
                    Console.WriteLine("Complete Child");
                    return 42;
                });
                Console.WriteLine("Complete parent");
                return child; // the Task return a Task<int> therefore t is Task<Task<int>>
            });

            t.ContinueWith((t1) => // t1 is Task<Task<int>> 
                    Write("Not realy dome (the top level task is immediately completed)", ConsoleColor.Yellow));
            t.Unwrap().ContinueWith(t2 => // t2 is Task<int>
                Write("Done", ConsoleColor.Green));
        }

        #endregion // CompletionTiming

        #region Write

        private static void Write(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        #endregion // Write
    }
}
