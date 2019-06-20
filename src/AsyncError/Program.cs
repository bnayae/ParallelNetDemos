using System;
using System.Threading.Tasks;

namespace AsyncError
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Continuation();
            // await SingleErrorAsync();
            await MultiErrorAsync();
            Console.ReadKey();


        }

        private static void Continuation()
        {
            Task t1 = Task.Run(() => throw new FieldAccessException("not so good"));
            Task t2 = Task.Run(() => throw new Exception("also not so good"));
            Task t3 = Task.WhenAll(t1, t2);
            Task fireForget = t3.ContinueWith(c =>
            {
                foreach (var e in c.Exception.Flatten().InnerExceptions)
                {
                    Console.WriteLine("--------------------------");
                    Console.WriteLine(e.GetBaseException().Message);
                }
                //Console.WriteLine(c.Exception);
            },
                TaskContinuationOptions.OnlyOnFaulted);
        }

        private static async Task SingleErrorAsync()
        {
            try
            {
                await Task.Run(() => Console.WriteLine("1"));// throw new FieldAccessException("not so good"));
                await Task.Run(() => Console.WriteLine(2));// throw new Exception("also not so good"));
                Console.WriteLine("#####");
                throw new Exception("bad");
            }
            catch (AggregateException e)
            {
                foreach (var ex in e.Flatten().InnerExceptions)
                {
                    Console.WriteLine("--------------------------");
                    Console.WriteLine(ex.GetBaseException().Message);
                }
            }
            catch (FieldAccessException e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
        }

        private static async Task MultiErrorAsync()
        {
            try
            {
                Task t1 = Task.Run(() => throw new FieldAccessException("not so good"));
                Task t2 = Task.Run(() => throw new Exception("also not so good"));
                await Task.WhenAll(t1, t2).ThrowAll();
                            //ContinueWith(c =>
                            //{
                            //    if (c.Exception != null)
                            //        throw c.Exception;
                            //});
                Console.WriteLine("#####");
            }
            catch (AggregateException e)
            {
                foreach (var ex in e.Flatten().InnerExceptions)
                {
                    Console.WriteLine("--------------------------");
                    Console.WriteLine(ex.GetBaseException().Message);
                }
            }
            catch (FieldAccessException e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
        }
    }
}
