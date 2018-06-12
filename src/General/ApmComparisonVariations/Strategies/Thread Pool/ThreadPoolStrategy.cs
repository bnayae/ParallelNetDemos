#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

#endregion // Using

namespace Tpl.Samples
{
    public class ThreadPoolStrategy: StrategyBase
    {
        /// <summary>
        /// Thread pool strategy
        /// </summary>
        /// <param name="degreeOfParallelism"></param>
        /// <param name="action"></param>
        public override void Execute(int degreeOfParallelism, Action<object> action)
        {
            for (int i = 0; i < degreeOfParallelism; i++)
            {
                int local = i;
                StartWork();
                ThreadPool.QueueUserWorkItem(state =>
                    {
                        action(state);
                        EndWork();
                    }, local);
            }

            Wait();
        }
    }
}
