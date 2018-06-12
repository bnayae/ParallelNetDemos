using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareAndSwapStack
{
    class Program
    {
        static void Main(string[] args)
        {
            var stack = new FreeStack<long>();

            // Push 1000
            Parallel.For (0, 1000, i => 
                {
                    stack.Push(i);
                });

            Console.WriteLine(stack.Count());

            // Pop 1000
            Parallel.For (0, 1000, i => 
                {
                    stack.Pop();
                });
            Console.WriteLine(stack.Count());

            // Push 1000 & Pop 500
            Parallel.For (0, 1000, i => 
                {
                    stack.Push(i);

                    if ((i % 2) == 0)
                        stack.Pop();
                });
            Console.WriteLine(stack.Count());

            Console.ReadKey();
        }
    }
}
