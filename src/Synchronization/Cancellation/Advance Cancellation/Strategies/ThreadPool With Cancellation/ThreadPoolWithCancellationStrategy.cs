#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

#endregion // Using

namespace Tpl.Samples
{
    /// <summary>
    /// Thread pool strategy with cancellation aware
    /// </summary>
    public class ThreadPoolWithCancellationStrategy : StrategyBase
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

            try
            {
                if (cancelToken.IsCancellationRequested) // check for cancellation
                {
                    sync.Signal();
                    return;
                }
            }
            #region Exception Handling

            catch (ObjectDisposedException)
            {
                Debug.WriteLine("Disposed");
            }

            #endregion // Exception Handling

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
