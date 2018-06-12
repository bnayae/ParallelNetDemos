#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

#endregion // Using

namespace Tpl.Samples
{
    /// <summary>
    /// Parallel Task strategy with cancellation aware
    /// </summary>   
    public class ParallelTaskWithCancellationStrategy : StrategyBase
    {
        public override void Find(Node node, int value, Action found, CountdownEvent sync)
        {
            CancellationToken cancelToken = _cancellationSource.Token;

            SpinWorkWithCancellation(cancelToken); // make the processor busy for short duration

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
                }, local, cancelToken); // cancelation token will prevent execution of un schedule task when if signaled
                tasks.Add(t);
            }

            sync.Signal();
        }
    }
}
