#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion // Using

namespace Sela.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

            //NotLazy(cts.Token);
            Lazy(cts.Token);

            Console.ReadKey();
        }

        #region NotLazy

        private static void NotLazy(CancellationToken token) 
        {

            Task a = Task.Run(() =>
            {
                Thread.Sleep(3000);
                Console.WriteLine("A");
            });

            Task b = a.ContinueWith(t => Console.WriteLine("B"), token);

            Task c = b.ContinueWith(t => Console.WriteLine("C"));
        }

        #endregion // NotLazy

        #region Lazy

        private static void Lazy(CancellationToken token) 
        {

            Task a = Task.Run(() =>
            {
                Thread.Sleep(3000);
                Console.WriteLine("A");
            });

            Task b = a.ContinueWith(t => Console.WriteLine("B"), token,
                 TaskContinuationOptions.LazyCancellation,
                 TaskScheduler.Default);

            Task c = b.ContinueWith(t => Console.WriteLine("C"));
        }

        #endregion // Lazy
    }
}
