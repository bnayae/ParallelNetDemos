#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

#endregion // Using

namespace ParentChildContinueation
{
    class Program
    {
        static void Main(string[] args)
        {
            var parentTask = Task.Factory.StartNew(() =>
            {
                Task childTask = Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("child1");
                }, TaskCreationOptions.AttachedToParent);

                childTask.ContinueWith(tsk =>
                {
                    Thread.Sleep(2000);
                    Console.WriteLine("child continuation");
                } /*, TaskContinuationOptions.AttachedToParent*/);
            });


            parentTask.ContinueWith(tsk => Console.WriteLine("parent continuation"));

            Console.ReadKey();
        }
    }
}
