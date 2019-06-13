using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCached
{
    class Program
    {
        private const string URL = "https://source.unsplash.com/3000x3000/?dog/";
        private static readonly  IDownloader _downloader = new Downloader();

        static void Main(string[] args)
        {

            Task<byte[]> t = _downloader.DownloadAsync(URL);
            Task t1 = t.ContinueWith(Callback);

            while (!t1.IsCompleted)
            {
                Console.Write($"A {t.IsCompleted}, ");
                Thread.Sleep(50);
            }
            Task<byte[]> t2 = _downloader.DownloadAsync(URL);
            Task t3 = t2.ContinueWith(c2 => File.WriteAllBytes("image2.jpg", c2.Result));
            while (!t3.IsCompleted)
            {
                Console.Write($"B {t2.IsCompleted}, ");
                Thread.Sleep(50);
            }

            Console.WriteLine("Done");
            Console.ReadKey();
        }


        private static void Callback(Task<byte[]> c)
        {
            // Thread.Sleep(500); // bad practice
            File.WriteAllBytes("image1.jpg", c.Result);
        }
    }
}
