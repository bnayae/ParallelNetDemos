using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskCached
{
    public interface IDownloader
    {
        Task<byte[]> DownloadAsync(string url);
    }
}
