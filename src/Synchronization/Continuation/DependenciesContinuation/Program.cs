#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

#endregion // Using

/// execute task which depend on other tasks

namespace Tpl.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random(42);
            var factory = new TaskFactory(TaskCreationOptions.AttachedToParent, TaskContinuationOptions.AttachedToParent);

            Task taskA = factory.StartNew((state) => DelayedWrite(500, state), "A");

            Task taskB = factory.StartNew((state) => DelayedWrite(1500, state), "B");

            Task taskC = factory.ContinueWhenAll(new[] { taskA, taskB }, (tasks) =>
                {
                    #region Write dependencies

                    Console.Write("'C' Depend on: ");
                    foreach (Task task in tasks)
                    {
                        Console.Write("{0} ", task.AsyncState);
                    }
                    Console.WriteLine(string.Empty);

                    #endregion // Write dependencies

                    int milliseconds = rnd.Next(500, 1500);
                    DelayedWrite(milliseconds, "C");
                });

            Task taskD = factory.StartNew((state) =>  DelayedWrite(rnd.Next(1500, 2500), state), "D");

            Task taskE = factory.ContinueWhenAny(new[] { taskC, taskD }, (task) =>
                {
                    #region Write dependency

                    Console.Write("'E' Depend on: ");
                    Console.WriteLine("{0} ", task.AsyncState == null ? "C" : task.AsyncState);

                    Console.WriteLine("Done");

                    #endregion // Write dependency
                });

            Console.ReadKey();
        }

        #region DelayedWrite

        private static void DelayedWrite(int milliseconds, object text)
        {
            Thread.Sleep(milliseconds);
            Console.WriteLine(text);
        }

        #endregion // DelayedWrite
    }
}
