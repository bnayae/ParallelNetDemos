#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion // Using

namespace AsyncWhenAll
{
    class Program
    {
        static void Main(string[] args)
        {
            Task t = ExecAsync();
            //Task t = ExecDeepAsync();

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        #region ExecAsync

        private static async Task ExecAsync()
        {
            try
            {
                Task t1 = BrokenMethod(new NullReferenceException(), 100);
                Task t2 = BrokenMethod(new ArgumentException(),3000);

                await t1;
                await t2;

                #region Option 1

                //await Task.WhenAll(t1, t2);

                #endregion // Option 1

                #region Better option

                //await Task.WhenAll(t1, t2)
                //        .ContinueWith(t =>
                //            {
                //                if (t.Exception != null)
                //                    throw t.Exception;
                //                if (t.IsCanceled)
                //                    throw new OperationCanceledException();
                //            });

                #endregion // Better option

                #region Best option

                //await Task.WhenAll(t1, t2).ThrowAll();

                #endregion // Best option
            }
            catch (AggregateException ex)
            {
                foreach (var exc in ex.Flatten().InnerExceptions)
                {
                    Console.WriteLine(exc.GetType().Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("## {0} ##", ex.GetType().Name);
            }
        }

        #endregion // ExecAsync

        #region ExecDeepAsync

        private static async Task ExecDeepAsync()
        {
            try
            {
                await Task.Run(async () =>
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
                        await Task.WhenAll(t11, t12);//.ThrowAll(); 
                    });
                    var t2 = Task.Run(() =>
                    {
                        throw new IndexOutOfRangeException("T2");
                    });
                    await Task.WhenAll(t1, t2);//.ThrowAll(); 
                });//.ThrowAll();
            }
            catch (AggregateException ex)
            {
                foreach (var exc in ex.Flatten().InnerExceptions)
                {
                    Console.WriteLine(exc.GetType().Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("## {0} ##", ex.GetType().Name);
            }
        }

        #endregion // ExecDeepAsync

        #region BrokenMethod

        private static async Task BrokenMethod(Exception ex, int sleep)
        {
            Console.WriteLine("Start BrokenMethod({0})", ex.GetType().Name);
            await Task.Delay(sleep);
            Console.WriteLine("Continue BrokenMethod({0})", ex.GetType().Name);
            throw ex;
        }

        #endregion // BrokenMethod
    }
}
