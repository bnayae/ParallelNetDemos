using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            var racers = Enumerable.Range(1, 5)
                    .Select(i => new Racer(ConsoleColor.Green + i, i));

            foreach (var racer in racers)
            {
                Thread t = new Thread(racer.Run);
                t.Start();
            }
            Console.ReadKey();
        }
    }
}
