using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class Racer
    {
        private readonly ConsoleColor _color;
        private readonly int _top;
        private readonly Random _rnd = new Random(Guid.NewGuid().GetHashCode());
        private static readonly object _gate = new object();

        public Racer(ConsoleColor color, int top)
        {
            _color = color;
            _top = top;
        }

        public void Run()
        {
            for (int i = 0; i < 40; i++)
            {
                Thread.Sleep(_rnd.Next(40, 300));
                Write(i);
            }
            Write(40, "|");
        }

        private void Write(int i, string sign = "->")
        {
            lock (_gate)
            {
                Console.CursorTop = _top;
                Console.ForegroundColor = _color;
                Console.CursorLeft = i;
                Console.Write(sign);
                Console.ResetColor();
            }
        }
    }
}
