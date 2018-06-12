using System;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics;
namespace Tpl.Samples
{
    public abstract class StrategyBase: IStrategy
    {
        private ManualResetEvent _sync = new ManualResetEvent(true);
        private static int _workCount = 0;

        #region Execute

        public abstract void Execute(int degreeOfParallelism, Action<object> action);

        #endregion // Execute

        #region Wait

        protected void Wait()
        {
            if (_workCount != 0)
                _sync.Reset();
            _sync.WaitOne();
        }

        #endregion // Wait

        #region EndWork

        protected void StartWork()
        {
            Interlocked.Increment(ref _workCount);
        }

        #endregion // EndWork

        #region EndWork

        protected void EndWork()
        {
            Interlocked.Decrement(ref _workCount);
            if (_workCount == 0)
                _sync.Set();
        }

        #endregion // EndWork

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        { 
             _sync.Dispose();
        }

        ~StrategyBase()
        {
            Dispose(false);
        }

        #endregion // Dispose
    }
}
