#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

#endregion // Using

namespace Tpl.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            const int ITERATIONS = 10000000;
            DateTime date = DateTime.Now;
            var anonymousResult = from item in ParallelEnumerable.Range(0, ITERATIONS)
                                  select new { Index = item, Date = date };         // reference type

            var structResult = from item in ParallelEnumerable.Range(0, ITERATIONS)
                               select new IndexDate { Index = item, Date = date };  // value type

            #region Warm-up

            var anonymousList = anonymousResult.Take(10000).ToList();
            var structList = structResult.Take(10000).ToList();

            Console.WriteLine("Warm-up {0}-{1}", anonymousList.Count, structList.Count);

            #endregion // Warm-up

            var sw = new Stopwatch();

            GC.Collect();
            sw.Restart();
            structList = structResult.ToList();
            sw.Stop();
            Console.WriteLine("Struct (Value type) duration = {0}, count = {1}", sw.ElapsedMilliseconds, structList.Count);
            
            GC.Collect();
            sw.Restart();
            anonymousList = anonymousResult.ToList();
            sw.Stop();
            Console.WriteLine("Anonymous (reference type) duration = {0}, count = {1}", sw.ElapsedMilliseconds, anonymousList.Count);

            Console.ReadKey();
        }
    }

    public struct IndexDate
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }

        // TODO: override GetHashCode and Equals
    }
}
