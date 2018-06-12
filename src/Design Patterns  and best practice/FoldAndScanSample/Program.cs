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
            const int SIZE = 20;
            var source = Enumerable.Range(1, SIZE).ToArray();
            var dest = new int[SIZE];
            dest[0] = source[0];

            #region Sync

            for (int i = 1; i < SIZE; i++)
            {
                dest[i] = dest[i - 1] + source[i];
            }

            #endregion // Sync

            // Scan
            var subResults = new ConcurrentBag<List<int>>();

            #region Documentation
            /*
             * public static ParallelLoopResult For<TLocal>(
             *      int fromInclusive, int toExclusive,
             *      Func<TLocal> localInit,
             *      Func<int, ParallelLoopState, TLocal, TLocal> body,
             *      Action<TLocal> localFinally);
             */
            #endregion // Documentation
            ParallelLoopResult result = Parallel.For<List<int>>(0, SIZE,
                () => new List<int>(), // seed - sub lists
                (i, state, subList) =>
                { // local thread pipeline (accumulate)
                    if (subList.Count == 0)
                        subList.Add(source[i]);
                    else
                        subList.Add(source[i] + subList[subList.Count - 1]);
                    return subList;
                }, // local thread pipeline
                (lst) => // merge all thread local pipelines
                {
                    subResults.Add(lst.ToList());
                });

            // Fold
            var orderedCollections = subResults.OrderBy(lst => lst.First()).ToArray(); // order by the first item in the sub collection
            
            var destList = orderedCollections.First(); // does not need to touch the first scan range
            int seedValue = destList[destList.Count - 1]; // last item in the local result

            // iterate on the local results
            for (int i = 1; i < orderedCollections.Length; i++)
            {
                IList<int> currentSubList = orderedCollections[i];
                var folded = from item in currentSubList
                                select item + seedValue; // adjust the local result  from previous result
                destList.AddRange(folded); // aggregate the local adjusted result
                seedValue += currentSubList[currentSubList.Count - 1]; // calculate the next seed 
            }

            for (int i = 0; i < destList.Count; i++)
            {
                Console.WriteLine("Sync = {0}, async = {1}", dest[i], destList[i]);
            }

            Console.ReadKey();
        }

    }
}
