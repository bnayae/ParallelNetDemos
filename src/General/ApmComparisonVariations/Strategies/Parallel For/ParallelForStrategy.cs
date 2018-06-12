#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion // Using

namespace Tpl.Samples
{
    public class ParallelForStrategy: StrategyBase
    {
        /// <summary>
        /// Parallel For strategy
        /// </summary>
        /// <param name="degreeOfParallelism"></param>
        /// <param name="action"></param>
        public override void Execute(int degreeOfParallelism, Action<object> action)
        {
            Parallel.For(0, degreeOfParallelism, state =>
            {
                action(state);
            });
        }
    }
}
