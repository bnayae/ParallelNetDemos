#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion // Using

/// demonstrate wrapping old API with TaskCompletionSource
/// which use to create the Task and Task completion
/// the implementation does not use the TPL scheduler, instead it use 
/// whatever a-sync technology implemented by it interception points 
/// like (result, exception and cancellation)

namespace Tpl.Samples
{
    class Program
    {
        // Any delegate can be invoke using APM (BeginInvoke / EndInvoke)
        private static Func<int, string> GetDelayedText = (milliseconds) =>
        {
            Thread.Sleep(milliseconds);
            return "Complete";
        };

        static void Main(string[] args)
        {
            Task<string> task = RunAsync(GetDelayedText);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Do work");
            Console.ResetColor();

            Console.WriteLine(task.Result);
            Console.ReadKey();
        }

        private static Task<string> RunAsync(Func<int, string> method)
        {
            var customAPM = new TaskCompletionSource<string>(); // use it for custom a-sync operations

            method.BeginInvoke(2000, ar => // handle completion
            {
                try
                {
                    string result = GetDelayedText.EndInvoke(ar);
                    customAPM.SetResult(result);
                }
                catch (Exception ex)
                {
                    customAPM.SetException(ex);
                }
            }, null);


            return customAPM.Task; // does not wait for completion
        }
    }
}
