#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;

#endregion // Using

/// the sample demonstrate parent child relationship
/// using the Wait operation

namespace Tpl.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press 'n' for non parent child execution");
            var c = Console.ReadKey(true);
            if (c.KeyChar == 'n')
            {
                #region Non parent child nested tasks

                #region Hide

                WriteColor(ConsoleColor.Yellow, "Non parent child nested tasks");
                var sw = Stopwatch.StartNew();

                #endregion // Hide

                Task t = Task.Factory.StartNew(state1 => // parent
                {
                    Task.Factory.StartNew(state2 => // level 1 not real child
                    {
                        Task.Factory.StartNew(state3 => // level 2 not real child
                        {
                            Task.Factory.StartNew(state4 => // level 3 not real child
                            {
                                SpinWork(4000, "not real child (level 3)", 10);
                            }, "Task 4");
                            SpinWork(3000, "not real child (level 2)", 20);
                        }, "Task 3");

                        SpinWork(2000, "not real child (level 1)", 30);
                    }, "Task 2");
                    SpinWork(1000, "parent", 40);
                    Debug.WriteLine("Done");
                }, "Task 1");

                #region Hide

                t.Wait(); // does not wait for the child task
                sw.Stop();
                WriteColor(ConsoleColor.White, "Expected duration = 4000, actual duration = {0}", sw.ElapsedMilliseconds);

                #endregion // Hide

                #endregion // Non parent child nested tasks
            }
            else
            {
                #region Parent child nested tasks

                #region Hide

                Console.WriteLine();
                WriteColor(ConsoleColor.Yellow, "Parent child nested tasks");
                var sw = Stopwatch.StartNew();

                #endregion // Hide


                Task t1 = Task.Factory.StartNew(() => // parent
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Task.Factory.StartNew(state2 => // level 1 child
                        {
                            Task.Factory.StartNew(state3 => // level 2 child
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    Task.Factory.StartNew(state4 => // level 3 child
                                    {
                                        SpinWork(4000, "child (level 3)", 40);
                                    }, "Task 4", TaskCreationOptions.AttachedToParent); // mark the new task as child task

                                    SpinWork(3000, "child (level 2)", 30);
                                }
                            }, "Task 3", TaskCreationOptions.AttachedToParent); // mark the new task as child task

                            SpinWork(2000, "child (level 1)", 20);
                        }, "Task 2", TaskCreationOptions.AttachedToParent); // mark the new task as child task
                        SpinWork(1000, "parent", 10);
                        Debug.WriteLine("Done");
                    }
                });

                #region Hide

                t1.Wait(); // wait for the child task
                WriteColor(ConsoleColor.White, "Expected duration = 4000, actual duration = {0}", sw.ElapsedMilliseconds);
                WriteColor(ConsoleColor.Gray, "Press any key");

                #endregion // Hide

             #endregion // Parent child nested tasks
            }
            Console.ReadKey();
        }

        #region SpinWork

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void SpinWork(int duration, string text, int sleepBefore = 0)
        {
            var sw = Stopwatch.StartNew();
            //Thread.Sleep(sleepBefore);
            while (sw.ElapsedMilliseconds < duration) ;
            sw.Stop();

            Console.WriteLine(text);
        }

        #endregion // SpinWork

        #region WriteColor

        private static void WriteColor(ConsoleColor color, string format, params object[] args)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(format, args);
            Console.ResetColor();
        }

        #endregion // WriteColor
    }
}