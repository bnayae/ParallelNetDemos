using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCached
{
    public class Downloader : IDownloader
    {
        private readonly HttpClient _http = new HttpClient();
        private readonly ConcurrentDictionary<string, Task<byte[]>> _cache = new ConcurrentDictionary<string, Task<byte[]>>();
        public Task<byte[]> DownloadAsync(string url)
        {
            if (_cache.TryGetValue(url, out Task<byte[]> result))
                return result;

            Task<byte[]> response = _http.GetByteArrayAsync(url);
            _cache.TryAdd(url, response);
            return response;
        }
    }
}
