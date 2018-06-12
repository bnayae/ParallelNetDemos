using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tpl.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskScheduler.UnobservedTaskException += (s, e) =>
                {
                    Console.WriteLine(e.Exception.ToString());
                    
                };

            //SimpleExceptionHandling();

            MultiExceptions();
            //ComplexExceptionHandling();
            Console.ReadKey();
        }

        #region SimpleExceptionHandling

        private static void SimpleExceptionHandling()
        {
            WriteColor("Simple", ConsoleColor.Yellow);
            Task t = Task.Factory.StartNew(() =>
            {
                throw new ArgumentNullException();
            });
            try
            {
                t.Wait(); // the exception will be thrown at Wait, Result or GC collect time if non of the previous happens (and nobody check for Exception after it has thrown)
            }
            catch (AggregateException ex)
            {
                foreach (Exception exc in ex.InnerExceptions)
                {
                    WriteColor(exc.GetType().Name, ConsoleColor.Red);
                }
            }
            WriteColor("---------------", ConsoleColor.Gray);
        }

        #endregion // SimpleExceptionHandling

        #region ComplexExceptionHandling

        private static void ComplexExceptionHandling()
        {
            Console.WriteLine();
            WriteColor("Parent child", ConsoleColor.Yellow);
            var t = Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Task.Factory.StartNew(() =>
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            Task.Factory.StartNew(() =>
                            {
                                throw new IndexOutOfRangeException();
                            }, TaskCreationOptions.AttachedToParent); // mark the new task as child task
                        }
                        throw new NotImplementedException();
                    }, TaskCreationOptions.AttachedToParent); // mark the new task as child task
                }
                throw new ArgumentNullException();
            });
            try
            {
                t.Wait(); // the exception will be thrown at Wait, Result or GC collect time if non of the previous happens (and nobody check for Exception after it has thrown)
            }
            catch (AggregateException ex)
            {
                AggregateException flattenException = ex.Flatten(); // remove nested AggregateException
                foreach (Exception exc in flattenException.InnerExceptions)
                {
                    WriteColor(exc.GetType().Name, ConsoleColor.Red);
                }
            }
        }

        #endregion // ComplexExceptionHandling

        #region MultiExceptions

        private static void MultiExceptions()
        {
            Console.WriteLine();
            var t = Task.Run(async () =>
            {
                var t1 = Task.Run(async () =>
                {
                    Task t11 = Task.Run(() =>
                    {
                        throw new NullReferenceException("T11");
                    });
                    Task t12 = Task.Run(() =>
                    {
                        throw new TimeoutException("T12");
                    });
                    await Task.WhenAll(t11, t12);
                });
                var t2 = Task.Run(() =>
                {
                    throw new IndexOutOfRangeException("T2");
                });
                await Task.WhenAll(t1, t2);
            });
            try
            {
                t.Wait(); // the exception will be thrown at Wait, Result or GC collect time if non of the previous happens (and nobody check for Exception after it has thrown)
            }
            catch (AggregateException ex)
            {
                AggregateException flattenException = ex.Flatten(); // remove nested AggregateException
                foreach (Exception exc in flattenException.InnerExceptions)
                {
                    WriteColor(exc.GetType().Name, ConsoleColor.Red);
                }
            }
        }

        #endregion // MultiExceptions

        #region WriteColor

        private static void WriteColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        #endregion // WriteColor
    }
}
