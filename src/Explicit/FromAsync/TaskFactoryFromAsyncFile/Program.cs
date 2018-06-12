#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

#endregion // Using

/// wrapping the APM (BeginInvoke / EndInvoke) with Task

namespace Tpl.Samples
{
    class Program
    {
        private const string FILE_NAME = "DynamicText.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Start");
		    #region Initialize a file

            if (!File.Exists(FILE_NAME))
            {
                byte[] buffer = Encoding.UTF8.GetBytes("Some Text \r\n".PadRight(1024, '*'));
                var sw = Stopwatch.StartNew();
                using (var file = File.OpenWrite(FILE_NAME))
                {
                    while (sw.ElapsedMilliseconds < 2)
                        file.Write(buffer, 0, buffer.Length);
                }
            }

		    #endregion // Initialize a file

            using (var file = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.Asynchronous))
            {
                byte[] buffer = new byte[file.Length];
                
                //Task<int> returns the number of bytes read
                Task<int> taskAsyncRead = Task<int>.Factory.FromAsync(
                        file.BeginRead,  // begin invoke delegate
                        file.EndRead,    // end invoke delegate
                        buffer,          // read buffer
                        0,               // start index
                        buffer.Length,   // length
                        null);           // optional state

                // handling the result with task continuation
                Task taskResult = taskAsyncRead.ContinueWith((tsk) =>
                    {
                        file.Close();
                        string data = Encoding.UTF8.GetString(buffer);
                        Console.WriteLine(data.Substring(0, 1200));
                    });

                #region Do Work

                Console.WriteLine("Working {0}", taskResult.IsCompleted);
                for (int i = 0; i < 100; i++)
                {
                    Console.Write(".");
                }

                #endregion // Do Work
            }

            Console.ReadKey();
        }
    }
}
