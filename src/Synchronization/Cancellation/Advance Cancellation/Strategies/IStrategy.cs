using System;
using System.Threading;
namespace Tpl.Samples
{
    public interface IStrategy : IDisposable
    {
        /// <summary>
        /// find value on the branch
        /// </summary>
        /// <param name="node">current node</param>
        /// <param name="value">value to find</param>
        /// <param name="found">find action</param>
        /// <param name="sync">wait object</param>
        void Find(Node node, int value, Action found, CountdownEvent sync);

        void Cancel();
        void Reset();
        CancellationToken Token { get; }
    }
}
