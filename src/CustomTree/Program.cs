using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTree
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");

            Node root = new Node(50);

            Parallel.For(0, 1000, i => root.Add(i));

            int count = root.Count();
            Console.WriteLine("Count = {0}", count);

            Console.ReadKey();
        }
    }
}
