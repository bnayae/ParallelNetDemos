using System;
using System.Threading;
namespace Tpl.Samples
{
    public interface IStrategy: IDisposable
    {
        void Execute(int degreeOfParallelism, Action<object> action);
    }
}
