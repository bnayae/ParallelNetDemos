#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

#endregion // Using

/// demonstrate wrapping old WebClient API with TaskCompletionSource
/// the implementation does not use the TPL scheduler, instead it using
/// the WebClient implementation

namespace Tpl.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string> task = DownloadAsync("http://blogs.microsoft.co.il/bnaya/");

            while (!task.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(50);
            }

            if (!task.IsCanceled && task.Exception != null)
            {
                Console.WriteLine(task.Result.Substring(0, 1000));
            }
            Console.ReadKey();
        }

        private static Task<string> DownloadAsync(string address)
        {
            var uri = new Uri(address);

            // present TAP semantics
            var tcs = new TaskCompletionSource<string>(); 

            var proxy = new WebClient();
            // handling the download async result
            proxy.DownloadStringCompleted += (s, e) => 
                {
                    if (e.Cancelled)
                        tcs.SetCanceled();
                    else if (e.Error != null)
                        tcs.SetException(e.Error);
                    else
                        tcs.SetResult(e.Result);

                    proxy.Dispose();
                };
            // start downloading a-sync
            proxy.DownloadStringAsync(uri); 

            return tcs.Task; // does not wait for completion
        }
    }
}
