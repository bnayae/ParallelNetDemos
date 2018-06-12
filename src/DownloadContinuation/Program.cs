#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

#endregion // Using

// before this sample, see the unwrap sample
// on latter chapter we will see simpler approach using async / await 

namespace Sela.Samples
{
    class Program
    {
        private const string IMG_REGEX = @"src=""(?<IMG>.+?.png)""";
        //private const string START_URL = "http://www.imdb.com/";
        private const string START_URL = "https://www.yahoo.com/";
        private static readonly Regex _imgRegex = new Regex(IMG_REGEX);

        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            if (!Directory.Exists("Images"))
                Directory.CreateDirectory("Images");

            StartDownloading();
            Console.ReadKey();
        }

        #region StartDownloading

        private static void StartDownloading()
        {
            Task downloading = DownloadImages();

            while (!downloading.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(10);
            }

            Console.WriteLine("\r\nComplete\r\n");
        }

        #endregion // StartDownloading

        #region DownloadImages

        private static Task DownloadImages()
        {
            var client = new WebClient();
            Task<string> htmlTask =
                client.DownloadStringTaskAsync(START_URL);

            // Dispose when done
            htmlTask.ContinueWith((t, state) => ((IDisposable)state).Dispose(), client);

            Task<Task> completion = htmlTask.ContinueWith(t =>
                {
                    IEnumerable<Task> downloading = DownloadImages(t.Result);
                    Console.WriteLine("\r\nDownloading in process\r\n");

                    Task all = Task.Factory.ContinueWhenAll(downloading.ToArray(), t1 => { }); // callback when all tasks completed
                    return all;
                });

            return completion.Unwrap(); // see the unwrap sample
        }

        #endregion // DownloadImages

        #region DownloadImages

        private static IEnumerable<Task> DownloadImages(string html)
        {
            string[] imageUrls = ParseImageTags(html);
            IEnumerable<Task> downloadingImages = // represent the process of downloading multiple images
                from url in imageUrls
                let httpClient = new WebClient() // doesn't support concurrent download
                let t = httpClient.DownloadDataTaskAsync(url)
                let disposing = t.ContinueWith(t1 =>
                      {
                          httpClient.Dispose();
                          return SaveImage(t1.Result, Path.GetFileName(url));
                      })
                select disposing.Unwrap(); // see the unwrap sample

            return downloadingImages;
        }

        #endregion // DownloadImages

        #region ParseImageTags

        private static string[] ParseImageTags(string html)
        {
            var output = new List<string>();
            try
            {
                var images = _imgRegex.Matches(html);
                foreach (Match item in images)
                {
                    var value = item.Groups["IMG"].Value;
                    output.Add(value);
                }
            }
            #region Exception Handling

            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error: {0}", ex.Message);
            }

            #endregion // Exception Handling
            return output.ToArray();
        }

        #endregion // ParseImageTags

        #region SaveImage

        private static Task SaveImage(byte[] buffer, string name)
        {
            string filename = @"Images\" + name;
            Stream target = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 4096,
                FileOptions.Asynchronous);
            Task completion = target.WriteAsync(buffer, 0, buffer.Length);

            // Dispose when done
            completion.ContinueWith((t, state) => ((IDisposable)state).Dispose(), target);
            return completion;
        }

        #endregion // SaveImage
    }
}
