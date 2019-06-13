using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class FileAsyncer
    {
        public Task<string> WatchFileDeletionAsync(string path)
        {
            // TODO: watch for deletion (return the file name)
            //throw new NotImplementedException();
            var tcs = new TaskCompletionSource<string>();
            var fsw = new FileSystemWatcher("Files");
            fsw.EnableRaisingEvents = true;
            fsw.Deleted += (e, a) => tcs.TrySetResult(a.Name);
            return tcs.Task;
        }
    }
}
