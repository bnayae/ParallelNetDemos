﻿#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion // Using

namespace Tpl.Samples
{
    public class ParallelTaskStrategy: StrategyBase
    {
        /// <summary>
        /// Parallel Task strategy
        /// </summary>
        /// <param name="degreeOfParallelism"></param>
        /// <param name="action"></param>
        public override void Execute(int degreeOfParallelism, Action<object> action)
        {
            for (int i = 0; i < degreeOfParallelism; i++)
            {
                int local = i;
                StartWork();
                Task.Factory.StartNew (state =>
                {
                    action(state);
                    EndWork();
                }, local);
            }

            Wait();
        }
    }
}
