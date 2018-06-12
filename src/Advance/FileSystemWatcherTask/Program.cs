#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

#endregion // Using

namespace Sela.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            //bool completed = false;

            var fsw = new FileSystemWatcher(".");

            //Task<string> t = FileSystemWatcherTask.ToTask(fsw, () =>
            //    Console.WriteLine("Executing"));
            Task<string> t = fsw.ToTask(() =>
                Console.WriteLine("Executing"));
            t.ContinueWith(tsk => 
                {
                    //completed = true;
                    Console.WriteLine();
                    Console.WriteLine(tsk.Result);
                });

            //while (!completed)
            while(!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }

            Console.ReadKey();
        }
    }
}
