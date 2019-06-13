using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            //var fsw = new FileSystemWatcher("Files");
            //fsw.EnableRaisingEvents = true;
            //fsw.Deleted += (e, a) => Console.WriteLine(a.Name);

            var w = new FileAsyncer();
            Task<string> t = w.WatchFileDeletionAsync("Files");
            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }
            Console.WriteLine(t.Result);

            Console.ReadKey();
        }
    }
}
