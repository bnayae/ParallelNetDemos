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
            
            // this is non GC friendly implementation 
            // the timer can be collect during the task work and is not explicitly disposed
            // better implementation will be present after the continuation model
            

            return tcs.Task;
        }
    }
}
