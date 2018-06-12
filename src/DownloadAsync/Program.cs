#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

#endregion // Using

// before this sample, see the unwrap sample

namespace Sela.Samples
{
    class Program
    {
        private const string IMG_REGEX = @"src=""(?<IMG>.+?.png)""";
        private const string START_URL = "http://www.imdb.com/";
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

        private async static Task DownloadImages()
        {
            string html;
            using (var client = new WebClient())
            {
                html = await client.DownloadStringTaskAsync(START_URL);
            } // Dispose when done

            IEnumerable<Task> downloading = DownloadImages(html);
            Console.WriteLine("\r\nDownloading in process\r\n");

            await Task.WhenAll(downloading);
        }

        #endregion // DownloadImagess

        #region DownloadImages

        private static IEnumerable<Task> DownloadImages(string html)
        {
            Func<string, Task> downloadAndSave = async url =>
                {
                    using(var httpClient = new WebClient())
                    {
                        byte[] buffer = await httpClient.DownloadDataTaskAsync(url);

                        try
                        {
                            await SaveImage(buffer, Path.GetFileName(url));
                        }
                        catch (ArgumentException)
                        {
                            Console.Write("X");
                        }
                    }
                };

            string[] imageUrls = ParseImageTags(html);
            IEnumerable<Task> downloadingImages = // represent the process of downloading multiple images
                from url in imageUrls
                    let t = downloadAndSave(url)
                    select t;

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

        private async static Task SaveImage(byte[] buffer, string name)
        {
            string filename = @"Images\" + name;
            using (Stream target = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 4096,
                FileOptions.Asynchronous))
            {
                await target.WriteAsync(buffer, 0, buffer.Length);              
            }  // Dispose when done
        }

        #endregion // SaveImage
    }
}
