using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1.AsyncExceptions
{
    class Program
    {
        static void Main(string[] args)
        {
            DoWork();

            Console.ReadKey();
        }

        static async void DoWork()
        {
            try
            {
                Console.WriteLine("Doing some complex work");
                //throw new Exception("Synchronic");

                await Task.Run(() =>
                {
                    Console.WriteLine("Complex work on other tread"); 
                    //throw new Exception("Task.Run");
                });
                //throw new Exception("Callback");
                Console.WriteLine("Finishing off");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex.Message);
            }
        }
    }
}
