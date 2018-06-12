using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ForAllSample
{
    class Program
    {
        static void Main(string[] args)
        {
            ParallelQuery<int> range = from item in Enumerable.Range(0, 1000).AsParallel()
                                       //.WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                                       //.WithMergeOptions(ParallelMergeOptions.FullyBuffered)
                                       where item % 2 == 0
                                       select Trace(item);


            //foreach (var item in range)
            //{
            //    Trace(item, "[{0}], "); // sequential
            //}

            #region Remarked

            //Parallel.ForEach(range, item => Trace(item, "[{0}], "));

            range.ForAll( item => Trace(item, "[{0}], "));

            #endregion // Remarked

            Console.ReadKey();
        }

        static int Trace(int i, string format = "{0}, ")
        {
            Console.Write(format, Thread.CurrentThread.ManagedThreadId);
            return i;
        }
    }
}
