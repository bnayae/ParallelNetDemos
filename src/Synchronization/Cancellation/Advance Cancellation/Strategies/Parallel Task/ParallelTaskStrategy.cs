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
    public class ParallelTaskStrategy : StrategyBase
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

            List<Task> tasks = new List<Task>();
            foreach (Node child in node.ChildNodes)
            {
                Node local = child;
                Task t = Task.Factory.StartNew((childState) =>
                {
                    Find((Node)childState, value, found, sync);
                }, local);
                tasks.Add(t);
            }
            sync.Signal();
        }
    }
}
