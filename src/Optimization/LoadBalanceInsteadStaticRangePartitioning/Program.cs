#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Threading;
using System.Collections.Concurrent;

#endregion // Using

// TODO: compare unbalance execution

namespace Tpl.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            // IList<T> will result with range partitioning
            var collection = Enumerable.Range(0, 1000).ToList();
            // loadbalancedPartitioner will use chunk partitioning instead of range partitioning
            var loadbalancedPartitioner = Partitioner.Create<int>(collection, true);
            Parallel.ForEach(loadbalancedPartitioner, range =>
            {
                // ...
            });
        }
    }
}
