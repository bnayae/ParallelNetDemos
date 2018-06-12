using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WebApiClient
{
    public class VM
    {
        private const string IMG_REGEX = @"""(?<IMG>http:.+?.png)""";
        private static readonly Regex _imgRegex = new Regex(IMG_REGEX, RegexOptions.Multiline);

        public VM()
        {
            Data = new ObservableCollection<byte[]>();
            Task t = DownloadAsync();
        }

        private async Task DownloadAsync()
        {
            using (var httpClient = new HttpClient())
            {
                //var response = await httpClient.GetAsync("http://localhost:5287/api/download");
                var response = await httpClient.GetAsync("http://www.imdb.com/");
                if (response.IsSuccessStatusCode)
                {
                    string html = await response.Content.ReadAsStringAsync();
                    var images = ParseImageTags(html);
                    foreach (var imgUrl in images)
                    {
                        try
                        {
                            var img = await httpClient.GetByteArrayAsync(imgUrl);
                            Data.Add(img);
                        }
                        catch (Exception ex)
                        {
                            Trace.Write(ex.ToString());
                        }
                    }
                }

            }
        }

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

        public ObservableCollection<byte[]> Data { get; set; }
    }
}
