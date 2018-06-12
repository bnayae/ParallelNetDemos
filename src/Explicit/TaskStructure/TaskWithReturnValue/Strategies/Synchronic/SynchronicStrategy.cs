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
    public class SynchronicStrategy: IStrategy
    {
        public int Sum(Node node)
        {
            int sum = node.GetValue();
            foreach (Node child in node.ChildNodes)
            {
                sum += Sum(child);

            }

            return sum;
        }
    }
}
