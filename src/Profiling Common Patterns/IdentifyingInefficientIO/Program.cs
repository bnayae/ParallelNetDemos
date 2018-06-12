#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.IO;
using System.Threading;

#endregion // Using

namespace Tpl.Samples
{
    class Program
    {
        private static readonly int ITERATIONS = 5 * Environment.ProcessorCount;
        private const int SPIN_MILLISECONDS = 10;
        private static readonly object _sync = new object();

        static void Main(string[] args)
        {
            Console.WriteLine("IO operation should execute a-sync using the IO threads");
            Console.WriteLine("[Check the profiler CPU view, the actual CPU usage]");
            Console.WriteLine("[Check the profiler Thread view, IO color]");
            Thread.Sleep(2000);

            var sw = Stopwatch.StartNew();
            var tempFiles = new List<string>();
            Parallel.For(0, ITERATIONS, i =>
            {
                string text = File.ReadAllText("SomeFile.txt");
                string fileName = Path.GetTempFileName();
                File.WriteAllText(fileName, text);
                tempFiles.Add(fileName);
            });

            Thread.Sleep(300);
            foreach (var fileName in tempFiles)
            {
                lock (_sync)
                {
                    File.Delete(fileName);
                }
            }

            Console.WriteLine("Duration = {0}", sw.ElapsedMilliseconds);
        }
    }
}
