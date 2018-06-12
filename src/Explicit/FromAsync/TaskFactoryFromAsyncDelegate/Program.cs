#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion // Using

/// Wrapping the APM (Asynchronous Programming Model [BeginInvoke / EndInvoke])
/// with Task, even though the underline scheduling does not handle by the TPL

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
		    #region Option 1

            Console.WriteLine("Option 1");

            Task<string> task1 = ExecFromIAsyncResult();

            WriteDoWork(1);
            Console.WriteLine("Result = {0}", task1.Result); // blocking operation

		    #endregion // Option 1

            #region Option 2

            Console.WriteLine("\r\nOption 2");
            Task<string> task2 = ExecFromDelegate("Option 2");

            WriteDoWork(2);
            Console.WriteLine("Result = {0}, State = {1}", task2.Result, task2.AsyncState);// blocking operation

            #endregion // Option 2

            #region Option 3

            Console.WriteLine("\r\nOption 3");
            Task<string> task31 = ExecFromDelegate("Option 3.1");
            Task<string> task32 = ExecFromDelegate("Option 3.2");

            Action<Task<string>> OnComplete = originTask =>
                Console.WriteLine("Result = {0}, State = {1}", originTask.Result, originTask.AsyncState);

            Task taskComplete31 = task31.ContinueWith(OnComplete);
            Task taskComplete32 = task32.ContinueWith(OnComplete);

            WriteDoWork(3);

            Task.WaitAll(taskComplete31, taskComplete32);
            Console.WriteLine("Done");


            #endregion // Option 3

            Console.ReadKey();
        }

        #region ExecFromIAsyncResult

        private static Task<string> ExecFromIAsyncResult()
        {
            var factory = new TaskFactory<string>();

            Task<string> task = factory.FromAsync(
                GetDelayedText.BeginInvoke(2000, null, null), // IAsyncResult
                GetDelayedText.EndInvoke);

            return task;
        }

        #endregion // ExecFromIAsyncResult

        #region ExecFromDelegate

        private static Task<string> ExecFromDelegate(string state)
        {
            var factory = new TaskFactory<string>();

            Task<string> task = factory.FromAsync(
                GetDelayedText.BeginInvoke,
                GetDelayedText.EndInvoke,
                2000, // parameter
                state); // state

            return task;
        }

        #endregion // ExecFromDelegate

        #region WriteDoWork

        private static void WriteDoWork(double optionNumber)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Option {0}: Do work", optionNumber);
            Console.ResetColor();
        }

        #endregion // WriteDoWork
    }
}
