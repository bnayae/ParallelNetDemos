#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion // Using

namespace BarrierSample
{
    class Program
    {
        private static ThreadLocal<int> _localCount = new ThreadLocal<int>();
        static void Main(string[] args)
        {
            const int AMOUNT = 3;
            Barrier _barrier = new Barrier(AMOUNT);
            for (int i = 0; i < AMOUNT; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    while (_localCount.Value < 10)
                    {
                        Console.Write(_localCount.Value);
                        _localCount.Value++;
                        _barrier.SignalAndWait();
                    }
                });
            }

            Console.ReadKey();
        }
    }    
}
