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

namespace Tpl.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            var partitioner = Partitioner.Create(0, 1000, 1000 / Environment.ProcessorCount);
            Parallel.ForEach(partitioner, range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    // ... process i without context switching
                }
            });
        }
    }
}
