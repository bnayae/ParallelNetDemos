using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public enum Info
{
    ID,
    Scheduler,
    Name,
    All
}

namespace Sela.Samples
{
    class Program
    {
        private static readonly CustomScheduler _scheduler = new CustomScheduler();
        static void Main(string[] args)
        {
            //Task t = ExecuteAsync();

            Console.ForegroundColor = ConsoleColor.White;
            var simple = new Simple();
            Task t = simple.ExecuteScenatio3Async();

            t.Wait();
            Console.WriteLine("Done");
            Console.ReadKey();
        }


        private static async Task ExecuteAsync()
        {
            await CombineExecution();
            Console.ReadKey();
        }

        private static async Task CombineExecution()
        {
            foreach (Info e in Enum.GetValues(typeof(Info)))
            {
                WriteThreadInfo("1. Start", ConsoleColor.White, e);
                await Task.Run(() => WriteThreadInfo("2. Task.Run", ConsoleColor.Magenta, e, 1));
                WriteThreadInfo("3. after await", ConsoleColor.White, e);

                Console.WriteLine("-------------------------");
                await Task.Factory.StartNew(() => ExecuteOnContextAsync(e),
                    CancellationToken.None, TaskCreationOptions.None,
                    _scheduler).Unwrap();
                Console.WriteLine("-------------------------");
                WriteThreadInfo("7. after wait ", ConsoleColor.White, e);
                await Task.Factory.StartNew(() => WriteThreadInfo("8. StartNew on Scheduler", ConsoleColor.Magenta, e, 1),
                    CancellationToken.None, TaskCreationOptions.None,
                    _scheduler);
                WriteThreadInfo("9. after await", ConsoleColor.White, e);

                Console.WriteLine("\r\nPress any key to continue");
                Console.ReadKey(true);
                Console.Clear();
            }
        }


        private static async Task ExecuteOnContextAsync(Info e)
        {
            WriteThreadInfo("4. Start with Schedule", ConsoleColor.Yellow, e, 1);
            await Task.Run(() => WriteThreadInfo("5. Task.Run", ConsoleColor.Green, e, 2));
            WriteThreadInfo("6. after await", ConsoleColor.Yellow, e, 1);
        }


        private static void WriteThreadInfo(string title, ConsoleColor color, Info info, int indent = 0)
        {
            string start = new string ('\t', indent);
            string cur = TaskScheduler.Current is CustomScheduler ? "Custom" : "ThreadPool";
            string def = TaskScheduler.Default is CustomScheduler ? "Custom" : "ThreadPool";

            Console.ForegroundColor = color;
            var thread = Thread.CurrentThread;
            switch (info)
            {
                case Info.ID:
                    Console.WriteLine("{0}{1}: ID = {2}",
                        start,
                        title,
                        thread.ManagedThreadId);
                    break;
                case Info.Scheduler:
                    Console.WriteLine("{0}{1}: Scheduler (Default = {2} Current = {3})",
                        start,
                        title,
                        def,
                        cur);
                    break;
                case Info.Name:
                    Console.WriteLine("{0}{1}: Name = {2}",
                        start,
                        title,
                        thread.Name ?? "Pool");
                    break;
                case Info.All:
                    Console.WriteLine("{0}{1}: {2} (Default = {3} Current = {4}) [ID = {5}]",
                        start,
                        title,
                        thread.Name ?? "Pool",
                        def,
                        cur,
                        thread.ManagedThreadId);
                    break;
            }
            Console.ResetColor();
        }
    }
}

/*
internal void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
{
	if (stateMachine == null)
	{
		throw new ArgumentNullException("stateMachine");
	}
	Thread currentThread = Thread.CurrentThread;
	ExecutionContextSwitcher executionContextSwitcher = default(ExecutionContextSwitcher);
	RuntimeHelpers.PrepareConstrainedRegions();
	try
	{
		ExecutionContext.EstablishCopyOnWriteScope(currentThread, false, ref executionContextSwitcher);
		stateMachine.MoveNext();
	}
	finally
	{
		executionContextSwitcher.Undo(currentThread);
	}
}
 */
