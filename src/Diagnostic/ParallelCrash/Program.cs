using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sela.Samples
{
    class Program
    {
        private static int _count = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Set Debug Diag to trace this process, then press any key");
            Console.ReadKey(true);
            Console.WriteLine("Start");
            Parallel.For(0, 30, A);
            Console.WriteLine("End");
        }

        private static void A(int i)
        {
            var s = new string('*', (i + 1));
            B(s);
        }

        private static void B(string i)
        {
            int count = Interlocked.Increment(ref _count);
            if (count % 3 == 0)
                C(count);
            else if (count % 3 == 1)
                D(count);
            else
                Task.Factory.StartNew(CEntry, count, TaskCreationOptions.AttachedToParent);
        }

        private static void CEntry(object count)
        {
            int cnt = (int) count;
            C(cnt);

            if (cnt > 20)
                throw new IndexOutOfRangeException("Producing a crash");
        }

        private static void C(int count)
        {
            Thread.Sleep(10);
            D(count);
        }

        private static void D(int count)
        {
            if (count % 2 == 0)
                E();
            else
                F();
        }

        private static void E()
        {
            Thread.Sleep(50);
            Console.Write(".");
            //Console.ReadKey();
        }

        private static void F()
        {
            Thread.Sleep(50);
            Console.Write(".");
            //Console.ReadKey();
        }
    }
}
