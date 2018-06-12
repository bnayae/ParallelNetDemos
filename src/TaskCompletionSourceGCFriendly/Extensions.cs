using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sela.Samples
{
    public static class Extensions
    {
        public static Task<double> Delay(this TimeSpan duration)
        {
            var tcs = new TaskCompletionSource<double>();
            var tmr = new Timer(state => 
                {
                    tcs.SetResult(duration.TotalMilliseconds);
                }, null,  duration, TimeSpan.Zero);

            #region GC keep alive and cleanup

            GCHandle handle = GCHandle.Alloc(tmr, GCHandleType.Normal); // prevent GC from collecting the Timer 
            tcs.Task.ContinueWith(originTask =>
                {
                    handle.Free();
                    tmr.Dispose();
                });

            #endregion // GC keep alive and cleanup

            return tcs.Task;
        }
    }
}
