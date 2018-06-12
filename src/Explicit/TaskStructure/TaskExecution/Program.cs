#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

#endregion // Using

/// the sample show how the TaskCreationOptions.LongRunning affect the task execution decision 
/// TaskCreationOptions.AttachedToParent and TaskCreationOptions.PreferFairness will be discuss later 

namespace Tpl.Samples
{
    class Program
    {
        private const int ITERATIONS = 60;

        static void Main(string[] args)
        {
            Console.WriteLine("1. Default execution");
            Console.WriteLine("2. Long Running execution");
            char key = Console.ReadKey().KeyChar;
            if (key == '1')
            {
                #region Default execution

                for (int i = 0; i < ITERATIONS; i++)
                {
                    int local = i;
                    Task t = new Task(state =>
                    {
                        if (state.Equals(0))
                            Console.WriteLine(Thread.CurrentThread.IsThreadPoolThread);
                        Thread.Sleep(550); // the task is waiting for long period
                        Console.Write("*");
                    }, local);
                    t.Start();
                }

                #endregion // Default execution
            }
            else
            {
                #region Long Running execution

                for (int i = 0; i < ITERATIONS; i++)
                {
                    int local = i;
                    Task t = new Task(state =>
                    {
                        if (state.Equals(0))
                            Console.WriteLine(Thread.CurrentThread.IsThreadPoolThread);
                        Thread.Sleep(550); // the task is waiting for long period
                        Console.Write("*");
                    }, local, TaskCreationOptions.LongRunning);
                    t.Start();
                }

                #endregion // Long Running execution
            }

            Console.ReadKey();
        }
    }
}
