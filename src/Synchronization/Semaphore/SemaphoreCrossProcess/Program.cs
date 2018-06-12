#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;

#endregion // Using

namespace Tpl.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowWidth = 40;
            const int LOCK_LIMIT = 3; 

            bool isCreated; // indicate whether the named semaphore was created or opened
            Console.WriteLine("Create semaphore: count = {0}", LOCK_LIMIT);
            using (var semaphore = new Semaphore(LOCK_LIMIT, LOCK_LIMIT, "CrossProc", out isCreated))
            {
                Console.WriteLine("Semaphore created = {0}", isCreated);
                //while (Console.ReadKey().KeyChar == 'r')
                //    semaphore.Release();
                for (int i = 0; i < LOCK_LIMIT; i++)
                {
                    // acquired single lock
                    semaphore.WaitOne(); // best practice to use timeout for avoiding potential deadlocks
                    Console.WriteLine("Enter {0}", i + 1);
                }

                if (isCreated)
                {
                    while (true)
                        ReleaseOnce(semaphore);
                }
               
                Console.WriteLine("Done");
            }

            Console.ReadKey();
       }

        private static void ReleaseOnce(Semaphore semaphore)
        {
            Console.WriteLine("Press any key to release");
            Console.ReadKey();

            semaphore.Release(); // release single lock 
            Console.WriteLine("Released");
        }
    }
}
