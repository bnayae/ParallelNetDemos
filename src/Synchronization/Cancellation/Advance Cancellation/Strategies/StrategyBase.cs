using System;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics;
namespace Tpl.Samples
{
    public abstract class StrategyBase: IStrategy
    {
        protected CancellationTokenSource _cancellationSource;

        /// <summary>
        /// find value on the branch
        /// </summary>
        /// <param name="node">current node</param>
        /// <param name="value">value to find</param>
        /// <param name="found">find action</param>
        /// <param name="sync">wait object</param>
        public abstract void Find(Node node, int value, Action found, CountdownEvent sync);

        public void Cancel()
        {
            _cancellationSource.Cancel();
        }

        public void Reset()
        {
            if (_cancellationSource != null)
                _cancellationSource.Dispose();
            _cancellationSource = new CancellationTokenSource();
        }

        public CancellationToken Token { get { return _cancellationSource.Token; } }

        #region SpinWorkWithCancellation

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public void SpinWorkWithCancellation(CancellationToken cancelToken)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < Program.SPIN_MILLISECONDS) // spin for the duration
            {
                try
                {
                    if (cancelToken.IsCancellationRequested) // check for cancellation
                        break;
                }
                #region Exception Handling

                catch (ObjectDisposedException)
                {
                    Debug.WriteLine("Disposed");
                }

                #endregion // Exception Handling
            } 
            sw.Stop();
        }

        #endregion // SpinWorkWithCancellation

        #region SpinWork

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public void SpinWork()
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < Program.SPIN_MILLISECONDS); // spin for the duration
            sw.Stop();
        }

        #endregion // SpinWork

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _cancellationSource.Dispose();
        }

        ~StrategyBase()
        {
            Dispose(false);
        }

        #endregion // Dispose
    }
}
