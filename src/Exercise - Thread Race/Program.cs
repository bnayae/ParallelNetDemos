using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var r = new Racer(ConsoleColor.Yellow, 3);
            //r.Run();
            var racers = Enumerable.Range(1, 5)
                            .Select(i => new Racer(ConsoleColor.Green + i, i));

            foreach (var racer in racers)
            {
                racer.Run();
            }

            Console.ReadKey();
        }

    }
}
