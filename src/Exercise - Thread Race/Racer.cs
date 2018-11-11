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

        public Racer(ConsoleColor color, int top)
        {
            _color = color;
            _top = top;
        }

        public void Run()
        {
            Console.CursorTop = _top;
            Console.ForegroundColor = _color;
            for (int i = 0; i < 40; i++)
            {
                Console.CursorLeft = i;
                Thread.Sleep(_rnd.Next(40, 300));
                Console.Write("->");
            }
            Console.CursorLeft = 40;
            Console.Write("|");
            Console.ResetColor();
        }
    }
}
