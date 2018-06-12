using System;
using System.Collections.Generic;
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
            Task t1 = Exec1();
            while (!t1.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(50);
            }
            Console.WriteLine("\r\n---------------------------------------------");
            Task t2 = Exec2();
            while (!t2.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(50);
            }
            Console.ReadKey();
        }

        private async static Task Exec1()
        {
            Console.WriteLine("Exec Start");
            await Task.Run(() => Thread.Sleep(500)); // Task.Run make it parallel -> await is a callback point indicator
            Console.WriteLine("Exec stage 1");
            await Task.Delay(500); // Task.Delay is better than Task.Run(() => Thread.Sleep(500)), no thread pool starvation
            Console.WriteLine("Exec stage 2");
        }

        private async static Task Exec2()
        {
            Console.WriteLine("Looking for future result");
            int i = await FutureResult(); // await is only a callback the new thread is open inside FutureResult()
            Console.WriteLine("Result = {0}", i);
        }

        private async static Task<int> FutureResult()
        {
            await Task.Delay(500);
            return 42; // return int (not Task<int>)
        }
    }
}
