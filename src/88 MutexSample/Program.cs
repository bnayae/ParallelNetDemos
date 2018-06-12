using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _88_MutexSample
{
    class Program
    {
        static void Main(string[] args)
        {
            StartWithOwnership();
            //StartPitfallAsync();
            //StartWithoutOwnership();

            Console.ReadKey();
        }

        private static void StartWithOwnership()
        {
            Console.WriteLine("Start");
            bool created;
            using (var mutex = new Mutex(true, "X", out created)) // ensure single execution
            {
                if (!created && !mutex.WaitOne(20000))
                    Console.WriteLine("Deadlock");
                else
                    Console.WriteLine("Work");

                Console.WriteLine("Pres key to release");
                Console.ReadKey();
                mutex.ReleaseMutex();
            }
            Console.WriteLine("Done");
        }

        private static void StartWithoutOwnership()
        {
            Console.WriteLine("Start");
            using (var mutex = new Mutex(false, "X")) // ensure single execution
            {
                if (!mutex.WaitOne(20000))
                    Console.WriteLine("Deadlock");
                else
                    Console.WriteLine("Work");

                Console.WriteLine("Pres key to release");
                Console.ReadKey();
                mutex.ReleaseMutex();
            }
            Console.WriteLine("Done");
        }

        private static async Task StartPitfallAsync()
        {
            bool created;
            using (var mutex = new Mutex(true, "X", out created)) // ensure single execution
            {
                try
                {
                    if (!created && !mutex.WaitOne(20000))
                    {
                        Console.WriteLine("Deadlock");
                    }
                    else
                        Console.WriteLine("Work");

                    Console.WriteLine("Pres key");
                    Console.ReadKey();
                    await Task.Delay(1);
                    mutex.ReleaseMutex();
                }
                #region Exception Handling

                catch (AbandonedMutexException ex)
                {
                    Console.WriteLine("Abandoned");
                }

                #endregion // Exception Handling
                finally
                {
                    try
                    {
                        mutex.ReleaseMutex();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(":(");
                    }
                }
            }
        }
    }
}
