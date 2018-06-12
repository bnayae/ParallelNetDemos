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
using System.IO;

#endregion // Using

/// credits: 
/// PATTERNS OF PARALLEL PROGRAMMING by 
/// Stephen Toub
/// Parallel Computing Platform
/// Microsoft Corporation

// this demo is calculating char count of file

namespace Tpl.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Option 1

            string data = File.ReadAllText(@"Contents\MapReduce from Wilipedia.txt");
            var charCount = MapReduce(
                data,
                source => source.ToArray(), // map
                c => c, // keySelector
                group => new[] { new KeyValuePair<char, int>(group.Key, group.Count()) });

            var result1 = charCount.ToList().OrderBy(p => p.Key);
            foreach (KeyValuePair<char, int> pair in result1.Take(10))
            {
                Console.WriteLine("Char '{0}' = {1}", pair.Key, pair.Value);
            }

            #endregion // Option 1

            #region Option 2

            #region Documentation
            /*
             * public static TResult Aggregate<TSource, TAccumulate, TResult>(
             *               this ParallelQuery<TSource> source,
             *               Func<TAccumulate> seedFactory,
             *               Func<TAccumulate, TSource, TAccumulate> updateAccumulatorFunc,
             *               Func<TAccumulate, TAccumulate, TAccumulate> combineAccumulatorsFunc,
             *               Func<TAccumulate, TResult> resultSelector);
             */
            #endregion // Documentation
            var result2 = data.ToArray().AsParallel().Aggregate(
                () => new SortedDictionary<char, int>(), // seed
                (dic, c) => { // local thread pipeline (accumulate)
                    if (dic.ContainsKey(c))
                        dic[c] = dic[c] + 1;
                    else
                        dic.Add(c, 1); 
                    return dic;
                },
                (dic1, dic2) => // merge all thread local pipelines (combine accumulates)
                {
                    foreach (var item in dic2)
	                {
                        if (dic1.ContainsKey(item.Key))
                            dic1[item.Key] += item.Value;
                        else
                            dic1.Add(item.Key, item.Value);
	                }
                    return dic1;
                },
                dic => dic); // select

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine();
            foreach (KeyValuePair<char, int> pair in result2.Take(10))
            {
                Console.WriteLine("Char '{0}' = {1}", pair.Key, pair.Value);
            }

            #endregion // Option 2

            Console.ReadKey();
        }

        private static ParallelQuery<TResult> MapReduce<TSource, TMapped, TKey, TResult>(
            TSource source,                                     // source
            Func<TSource, IEnumerable<TMapped>> map,              // break the the units into separate units
            Func<TMapped, TKey> keySelector,                    // calculate the key from a unit
            Func<IGrouping<TKey, TMapped>, IEnumerable<TResult>> reduce) // calculate groups into 
        {
            var mapped = map(source).AsParallel().WithMergeOptions(ParallelMergeOptions.FullyBuffered); // mapped.OrderBy(p => p.Key).ToArray()
            var group = mapped.GroupBy(keySelector); // group.ToArray()
            var reducedResult = group.SelectMany(reduce); // reducedResult.ToArray()
            return reducedResult;
        }
    }
}
