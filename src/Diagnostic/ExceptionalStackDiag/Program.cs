using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sela.Samples
{
    class Program
    {
        private static int _count = 0;
        private const bool THROW = true;
        private const int FOR_COUNT = 30;
        private const int WORK_DURATION_SECONDS = 2;
        private static readonly CountdownEvent _sync = new CountdownEvent(FOR_COUNT);
        private static readonly object _syncWrite = new object();

        static void Main(string[] args)
        {
            while (true)
            {
                _sync.Reset();

                Write("\r\nPress any key to start\r\n", ConsoleColor.Green, "{0}");
                Console.ReadKey(true);
                Parallel.For(0, FOR_COUNT, A);
                _sync.Wait();
                Write("\r\nComplete\r\n", ConsoleColor.Green, "{0}");
            }
        }

        private static void A(int i)
        {
            Write("A", ConsoleColor.White);
            var s = new string('*', (i + 1));
            B(s);
            Write("A", ConsoleColor.Yellow);
        }

        private static void B(string i)
        {
            Write("B", ConsoleColor.White);
            DoWork();
            int count = Interlocked.Increment(ref _count);
            if (count % 3 == 0)
                C(count);
            else if (count % 3 == 1)
                D(count);
            else
            {
                Task.Factory.StartNew(() => C(count), TaskCreationOptions.AttachedToParent);
            }
            Write("B", ConsoleColor.Yellow);
        }

        private static void C(int count)
        {
            Write("C", ConsoleColor.White);
            DoWork();
            D(count);
            Write("C", ConsoleColor.Yellow);
        }

        private static void D(int count)
        {
            Write("D", ConsoleColor.White);
            if (count % 2 == 0)
                E();
            else
                F();
            Write("D", ConsoleColor.Yellow);
        }

        private static void E()
        {
            Write("E", ConsoleColor.White);
            DoWork();
            Write("E", ConsoleColor.Yellow);
            _sync.Signal();
        }

        private static void F()
        {
            Write("F", ConsoleColor.White);
            DoWork();
            Write("F", ConsoleColor.Yellow);
            _sync.Signal();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void DoWork()
        {
            int durationMilliseconds = WORK_DURATION_SECONDS * 1000;
            if (Environment.TickCount % 3 == 0)
                durationMilliseconds /= 10;
            var sw = Stopwatch.StartNew(); try
            {
                bool flag = false;
                while (sw.ElapsedMilliseconds < durationMilliseconds)
                {
                    if (sw.ElapsedMilliseconds % 100 == 0)
                    {
                        if (!flag)
                        {
                            flag = true;
                            Write(".", ConsoleColor.Gray, "{0}");
                        }

                        if (Environment.TickCount % 100 == 0 && THROW)
                            throw new ArgumentException("Some Error");
                    }
                    else
                        flag = false;
                }
                sw.Stop();
            }
            catch (Exception ex)
            {
                Write(ex.Message, ConsoleColor.Red);
            }
        }

        private static void Write(string message, ConsoleColor color = ConsoleColor.Gray, string pattern = "{0},")
        {
            lock (_syncWrite)
            {
                Console.ForegroundColor = color;
                Console.Write(pattern, message);
                Console.ResetColor();
            }
        }
    }
}
