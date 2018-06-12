#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion // Using

namespace Tpl.Samples
{
    /// <summary>
    /// Parallel Task strategy
    /// </summary>
    public class ParallelTaskStrategy : IStrategy
    {
        public int Sum(Node node)
        {
            List<Task<int>> results = new List<Task<int>>();
            foreach (Node child in node.ChildNodes)
            {
                Node local = child;
                Func<object, int> SumFunc =  (o) => Sum((Node)o);

                // create new task and start executing 
                // other options:
                // Task<int> t = new Task<int>(SumFunc, local);
                // t.Start();
                // C# 5 will use Task.Run(...)
                Task<int> t = Task.Factory.StartNew<int>(SumFunc, local); // async call (the result will be kept in the Task<int> structure) 

                results.Add(t);
            }

            #region Aggregate the results

            int sum = node.GetValue();
            foreach (Task<int> t in results)
            {
                sum += t.Result; // blocking property (wait for the result or cancelation)
            }

            #endregion // Aggregate the results

            return sum;
        }
    }
}
