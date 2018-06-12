#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

#endregion // Using

namespace Tpl.Samples
{
    /// <summary>
    /// Thread pool strategy
    /// </summary>
    public class ThreadPoolStrategy : StrategyBase
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

            int countdown = node.ChildNodes.Length;
            foreach (Node child in node.ChildNodes)
            {
                Node local = child;
                ThreadPool.QueueUserWorkItem((childState) =>
                {
                    Find((Node)childState, value, found, sync);
                }, local);
            }        

            sync.Signal();
        }
    }
}
