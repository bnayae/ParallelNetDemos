#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#endregion // Using

/// Execute IO or remote operation in parallel with CPU intensive operation

namespace Tpl.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("MAKE SURE THAT TplSyndicationServiceHost.exe IS RUNNING !!!");
            Console.ResetColor();

            const string URI = "http://localhost:8080/Tpl.Samples/TplFeed/";
            const int SPIN_DURATION = 1000;

            // warm up
            Download(URI);

            var sw = Stopwatch.StartNew();
            string rss = Download(URI);
            SpinWork(SPIN_DURATION);
            sw.Stop();
            Console.WriteLine("Sync:    {0}", sw.ElapsedMilliseconds);

            sw.Restart();
            Parallel.Invoke(
                () => Download(URI),
                () => SpinWork(SPIN_DURATION));
            sw.Stop();
            Console.WriteLine("A-Sync:  {0}", sw.ElapsedMilliseconds);

            Console.ReadKey();
        }

        #region Download

        private static string Download(string address)
        {
            var uri = new Uri(address);
            var proxy = new WebClient();
            string result = proxy.DownloadString(uri); // start downloading sync
            return result;
        }

        #endregion // Download

        #region SpinWork

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void SpinWork(int duration)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < duration) ; // spin for the duration
            sw.Stop();
        }

        #endregion // SpinWork
    }
}
