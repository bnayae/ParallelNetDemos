#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

#endregion // Using

namespace Tpl.Samples
{
    class Program
    {        
        static void Main(string[] args)
        {
            // the cancellation source can signal cancellation
            var cancelSource = new CancellationTokenSource();
            CancellationToken token = cancelSource.Token;
            var options = new ParallelOptions { CancellationToken = token };

            Task.Factory.StartNew(() => // execute the Parallel.For on other thread
                {
                    Parallel.For(0, 105, options, (i, parallelState) =>
                        {
                            Thread.Sleep(1000);

                            Console.WriteLine("Scheduled tasks does not canceled automatically");
                            if (parallelState.ShouldExitCurrentIteration)
                            {
                                parallelState.Stop();
                                return;
                            }
                            Console.WriteLine("Should not reach this point");
                        });
                });

            Thread.Sleep(200);
            cancelSource.Cancel(); // doesn't canceled scheduled tasks automatically

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
