using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TaskCached
{
    public class Downloader : IDownloader
    {
        private readonly HttpClient _http = new HttpClient();

        public Task<byte[]> DownloadAsync(string url)
        {
            Task<byte[]> response = _http.GetByteArrayAsync(url);
            return response;
        }
    }
}
