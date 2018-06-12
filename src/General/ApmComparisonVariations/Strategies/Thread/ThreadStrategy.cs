#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

#endregion // Using

namespace Tpl.Samples
{
    public class ThreadStrategy: StrategyBase
    {
        /// <summary>
        /// Thread strategy
        /// </summary>
        /// <param name="degreeOfParallelism"></param>
        /// <param name="action"></param>
        public override void Execute(int degreeOfParallelism, Action<object> action)
        {
            for (int i = 0; i < degreeOfParallelism; i++)
            {
                int local = i;
                StartWork();
                new Thread (state =>
                {
                    action(state);
                    EndWork();
                }).Start(local);
            }

            Wait();
        }
    }
}
