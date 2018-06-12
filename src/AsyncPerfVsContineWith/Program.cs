#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion // Using

namespace AsyncPerfVsContineWith
{
    class Program
    {
        private const int ITERATIONS = 100000;
        static void Main(string[] args)
        {
            #region Warm-up

            DoAwaits(2).Wait();
            DoContinuations(2).Wait();

            #endregion // Warm-up

            Measure("Multiple continuation call to [Task.Run(() => { })]",
                DoAwaits, DoContinuations);

            Measure(@"Multiple continuation call to [TaskCompletionSource], 
    everything goes on the same thread",
                DoAwaitsConceptual, DoContinuationsConceptual);

//            Measure(@"Multiple continuation call to [TaskCompletionSource], 
//    everything goes on the same thread",
//                DoAwaitsConceptual, DoContinuationsConceptual, DoContinuationsConceptualWithoutUnwrap, "with unwrapping", "Without unwrapping");

            Measure(@"Multiple continuation call to [Task.FromResult(42)], 
    everything goes on the same thread",
                DoAwaitsFromResult, DoContinuationsFromResult);

        }

        #region AsyncMethod

        private static Task AsyncMethod()
        {
            return Task.Factory.StartNew(() => { });
        }

        #endregion // AsyncMethod


        #region DoAwaits

        private static async Task DoAwaits(int ntasks)
        {
            for (int i = 0; i < ntasks; i++)
                await Task.Run(() => { });
        }

        #endregion // DoAwaits

        #region DoContinuations

        private static Task DoContinuations(int ntasks)
        {
            Task curr = AsyncMethod();
            for (int i = 1; i < ntasks; i++)
            {
                curr = curr.ContinueWith(_ =>
                    Task.Run(() => { }))
                    .Unwrap(); // extract the result from the task wrapper
            }
            return curr; 
        }

        #endregion // DoContinuations


        #region DoAwaitsConceptual

        private static async Task DoAwaitsConceptual(int ntasks)
        {
            for (int i = 0; i < ntasks; i++)
                await ConceptualAsyncMethod();
        }

        #endregion // DoAwaitsConceptual

        #region DoContinuationsConceptual

        private static Task DoContinuationsConceptual(int ntasks)
        {
            Task curr = ConceptualAsyncMethod();
            for (int i = 1; i < ntasks; i++)
                curr = curr.ContinueWith(_ => ConceptualAsyncMethod()).Unwrap(); return curr; 
        }

        #endregion // DoContinuationsConceptual

        #region DoContinuationsConceptualWithoutUnwrap

        private static Task DoContinuationsConceptualWithoutUnwrap(int ntasks)
        {
            Task curr = ConceptualAsyncMethod();
            for (int i = 1; i < ntasks; i++)
                curr = curr.ContinueWith(t => {/* empty */ }); 
            return curr; 
        }

        #endregion // DoContinuationsConceptualWithoutUnwrap

        #region ConceptualAsyncMethod

        private static Task ConceptualAsyncMethod()
        {
            var t = new TaskCompletionSource<object>();
            t.SetResult(null);
            return t.Task;
        }

        #endregion // ConceptualAsyncMethod


        #region DoAwaitsFromResult

        private static async Task DoAwaitsFromResult(int ntasks)
        {
            for (int i = 0; i < ntasks; i++) 
                await Task.FromResult(42);
        }

        #endregion // DoAwaitsFromResult

        #region DoContinuationsFromResult

        private static Task DoContinuationsFromResult(int ntasks)
        {
            Task curr = Task.FromResult(42);
            for (int i = 1; i < ntasks; i++)
                curr = curr.ContinueWith(t => Task.FromResult(42)); 
            return curr; 
        }

        #endregion // DoContinuationsFromResult

        #region Measure

        private static void Measure(
            string title, 
            Func<int, Task> awaitImplementation,
            Func<int, Task> oldImplementation1,
            Func<int, Task> oldImplementation2 = null,
            string subTitle1 = null,
            string subTitle2 = null)
        {

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(title);
            Console.ResetColor();

            DoGCCollect();

            var startBytes = GC.GetTotalMemory(false);
            var sw = Stopwatch.StartNew();
            awaitImplementation(ITERATIONS).Wait();
            sw.Stop();
            var endBytes = GC.GetTotalMemory(false);
            Console.WriteLine("\tawait:         {0:0000}, allocations = {1:0000000000}",
                sw.ElapsedMilliseconds, endBytes - startBytes);

            DoGCCollect();

            sw.Restart();
            oldImplementation1(ITERATIONS).Wait();
            sw.Stop();
            endBytes = GC.GetTotalMemory(false);
            if (subTitle1 != null)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\t{0}", subTitle1);
                Console.ResetColor();
            }
            Console.WriteLine("\tcontinuations: {0:0000}, allocations = {1:0000000000}",
                sw.ElapsedMilliseconds, endBytes - startBytes);

            if (oldImplementation2 != null)
            {
                DoGCCollect();

                sw.Restart();
                oldImplementation2(ITERATIONS).Wait();
                sw.Stop();
                endBytes = GC.GetTotalMemory(false);
                if (subTitle2 != null)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\t{0}", subTitle2);
                    Console.ResetColor();
                }
                Console.WriteLine("\tcontinuations: {0:0000}, allocations = {1:0000000000}",
                    sw.ElapsedMilliseconds, endBytes - startBytes);
            }
            Console.WriteLine();
        }

        #endregion // Measure

        #region DoGCCollect

        private static void DoGCCollect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        #endregion // DoGCCollect
    }
}
