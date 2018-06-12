using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BarrierCalculation
{
    class Program
    {
        private const int LENGTH = 10;
        private const int THREAD_COUNT = 3;

        private static int[,] _arr = new int[THREAD_COUNT, LENGTH];
        // thread safe random
        private static readonly ThreadLocal<Random> _rnd = 
            new ThreadLocal<Random>( () => new Random(Guid.NewGuid().GetHashCode())); 
        // thread synchronization
        private static Barrier _barrier = new Barrier(THREAD_COUNT);

        static void Main(string[] args)
        {
            // parallel calculation
            Parallel.For (0, THREAD_COUNT, Calc);

            #region Console.Write (_arr)

            for (int j = 0; j < THREAD_COUNT; j++)
            {
                for (int i = 0; i < LENGTH; i++)
                {
                    Console.Write("{0},\t", _arr[j,i]);
                }
                Console.WriteLine();
            }

            #endregion // Console.Write (_arr)

            Console.ReadKey();
        }

        /// <summary>
        /// calculation unit
        /// </summary>
        /// <param name="threadIndex"></param>
        private static void Calc(int threadIndex)
        {
            for (int i = 1; i < LENGTH; i++)
            {
                #region var sum = previous values produced by all threads

                var sum = 0; // sum depend on the other thread calculation
                for (int j = 0; j < THREAD_COUNT; j++)
                {
                    sum += _arr[j, i - 1];
                }

                #endregion // var sum = previous values produced by all threads

                _arr[threadIndex, i] = _rnd.Value.Next(10) + sum;
                _barrier.SignalAndWait(); // wait for other threads
            }            
        }
    }
}
