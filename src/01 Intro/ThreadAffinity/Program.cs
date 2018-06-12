using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Permissions;

namespace ThreadAffinity
{
    class Program
    {
        static void Main(string[] args)
        {
            ControlAffinity.SetProcAffinity(0,2);

            #region Console.WriteLine

            Console.WriteLine("Thread");
            Console.WriteLine();

            #endregion // Console.WriteLine

            int iterations = ExecThread();

            #region Console.WriteLine

            Console.WriteLine();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey(true);

            Console.WriteLine();
            Console.WriteLine(new string('-', 80));
            Console.WriteLine();

            #endregion // Console.WriteLine

            ExecTask(iterations);

            #region Console.WriteLine

            Console.WriteLine();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey(true);

            Console.WriteLine();
            Console.WriteLine(new string('-', 80));
            Console.WriteLine();
            Console.WriteLine("Parallel.For");
            Console.WriteLine();
            GCcollect();

            #endregion // Console.WriteLine

            Parallel.For(0, iterations, i => ControlAffinity.Run());

            
            Console.ReadKey();
        }

        #region ExecThread

        private static int ExecThread()
        {
            int iterations = Environment.ProcessorCount * 3;
            Thread[] trd = new Thread[iterations];
            for (int i = 0; i < iterations; i++)
            {
                trd[i] = new Thread(ControlAffinity.Run);
                //trd[i] = new Thread(ControlAffinity.RunWithWin32AffinityMask);
                trd[i].Start();
            }
            for (int i = 0; i < iterations; i++)
            {
                trd[i].Join();
            }
            return iterations;
        }

        #endregion // ExecThread

        #region ExecTask

        private static void ExecTask(int iterations)
        {
            Task[] tsk = new Task[iterations];
            for (int i = 0; i < iterations; i++)
            {
                tsk[i] = Task.Factory.StartNew(ControlAffinity.Run);
            }

            Task.WaitAll(tsk);
        }

        #endregion // ExecTask


        #region GCcollect

        private static void GCcollect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        #endregion // GCcollect
    }
}
