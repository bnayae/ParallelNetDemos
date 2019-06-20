using System;
using System.IO;

namespace Exercise___Async_Loop
{
    class Program
    {
        private const string URL = "https://source.unsplash.com/1200x1200";
        private const string URL_WITH_TOPIC = "https://source.unsplash.com/1200x1200/?{1}/";

        static void Main(string[] args)
        {
            // execute 100 iterations (with cancellation after 200 ms) 

            // 1. sequential 
            // 2. all in parallel
            // 3. with batch = 3
        }
    }
}
