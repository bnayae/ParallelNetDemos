using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sela.Samples
{
    public class Service
    {
        public static async Task<int> ExecuteAsync()
        {
            await Task.Delay(1000); //.ConfigureAwait(false);

            // UI thread?
            Trace.WriteLine("DONE");
            return 42;
        }
    }
}
