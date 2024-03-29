﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sela.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            Task<double> t = TimeSpan.FromSeconds(2).Delay();
            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(100);

                // DO NOT USE GC.Collect() in production code (it is just for the demo)
                // run it without debugger, compiled in release mode
                GC.Collect(); 
            }
            Console.WriteLine("\r\nComplete: {0}", t.Result);
            Console.ReadKey();
        }
    }
}
