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
    /// BeginInvoke Strategy strategy
    /// </summary>
    public class BeginInvokeStrategy : IStrategy
    {
        public int Sum(Node node)
        {
            Func<object, int> method = (o) => Sum((Node)o);

            List<IAsyncResult> results = new List<IAsyncResult>();
            foreach (Node child in node.ChildNodes)
            {
                Node local = child;
                IAsyncResult result = method.BeginInvoke(local, null, null); // async call 
                results.Add(result);
            }

            #region Aggregate the results

            int sum = node.GetValue();
            foreach (IAsyncResult result in results)
            {
                sum += method.EndInvoke(result); // blocking property (wait for the result or cancelation)
            }

            #endregion // Aggregate the results

            return sum;
        }

    }
}
