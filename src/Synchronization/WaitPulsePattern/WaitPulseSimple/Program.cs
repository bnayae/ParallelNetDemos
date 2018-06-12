using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WaitPulseSimple
{
    class Program
    {
        static readonly object _sync = new object();
        static bool _quit;

        static void Main(string[] args)
        {
            for (int i = 0; i < 7; i++)
            {
                Task.Factory.StartNew(Work, TaskCreationOptions.LongRunning);
            }

            Console.WriteLine("s - for single pulse");
            Console.WriteLine("a - for pulse all");
            Console.WriteLine("q - for quit");

            ConsoleKeyInfo info;
            do
            {
                info = Console.ReadKey();
                lock (_sync)
                {
                    switch (info.KeyChar)
                    {
                        case 's':
                            Monitor.Pulse(_sync);
                            break;
                        case 'a':
                            Monitor.PulseAll(_sync);
                            break;
                        case 'q':
                            _quit = true;
                            Monitor.PulseAll(_sync);
                            break;

                    }
                } // Monitor.Wait is released if Monitor.Pulse / Monitor.PulseAll was called
            } while (info.KeyChar != 'q');

            Thread.Sleep(10000);
        }

        static void Work()
        {
            while (!_quit)
            {
                lock (_sync)
                {
                    Monitor.Wait(_sync); // Lock is released until Monitor.Pulse
                    Thread.Sleep(1000);  // lock is regained
                }
                Console.WriteLine("\r\nWrok: {0}", Thread.CurrentThread.ManagedThreadId);
            }

            Console.WriteLine("Stop: {0}", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
