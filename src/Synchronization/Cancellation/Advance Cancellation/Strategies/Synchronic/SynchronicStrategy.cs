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
    /// Synchronic strategy
    /// </summary>
    public class SynchronicStrategy : StrategyBase
    {
        public override void Find(Node node, int value, Action found, CountdownEvent sync)
        {
            SpinWork(); // make the processor busy for short duration

            if (node.Value == value)
            {
                found();
                sync.Signal();
                return;
            }

            foreach (Node child in node.ChildNodes)
            {
                Node local = child;
                Find(child, value, found, sync);
            }
            sync.Signal();
        }
    }
}
