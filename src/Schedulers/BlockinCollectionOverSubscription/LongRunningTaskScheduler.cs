using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace Bnaya.Samples
{
    public sealed class LongRunningTaskScheduler:TaskScheduler, IDisposable
    {
        private readonly ConcurrentQueue<Task> _requests = new ConcurrentQueue<Task>();
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
        private readonly ManualResetEventSlim _sync = new ManualResetEventSlim(false);
        private int _counter = 0;

        #region Ctor

        public LongRunningTaskScheduler(int maxDegreeOfParallism)
        {
            for (int i = 0; i < maxDegreeOfParallism; i++)
            {
                Task.Factory.StartNew(Execute, TaskCreationOptions.LongRunning);
            }
        }

        #endregion // Ctor

        #region Execute

        /// <summary>
        /// Executes requests.
        /// </summary>
        private void Execute()
        { 
            var cancelToken = _cancellation.Token;
            while (!cancelToken.IsCancellationRequested)
            {
                _sync.Wait(cancelToken);
                Task t;
                while(_requests.TryDequeue(out t))
                    base.TryExecuteTask(t);
                _sync.Reset();
            }
        }

        #endregion // Execute

        #region QueueTask

        /// <summary>
        /// Queues a <see cref="T:System.Threading.Tasks.Task"/> to the scheduler.
        /// </summary>
        /// <param name="task">The <see cref="T:System.Threading.Tasks.Task"/> to be queued.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="task"/> argument is null.</exception>
        protected override void QueueTask(Task task)
        {
            _requests.Enqueue(task);
            _sync.Set();
            int c = Interlocked.Increment(ref _counter);
            Console.Write("{0}, ", c);
        }

        #endregion // QueueTask

        #region TryExecuteTaskInline

        /// <summary>
        /// Determines whether the provided <see cref="T:System.Threading.Tasks.Task"/> can be executed synchronously in this call, and if it can, executes it.
        /// </summary>
        /// <param name="task">The <see cref="T:System.Threading.Tasks.Task"/> to be executed.</param>
        /// <param name="taskWasPreviouslyQueued">A Boolean denoting whether or not task has previously been queued. If this parameter is True, then the task may have been previously queued (scheduled); if False, then the task is known not to have been queued, and this call is being made in order to execute the task inline without queuing it.</param>
        /// <returns>
        /// A Boolean value indicating whether the task was executed inline.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="task"/> argument is null.</exception>
        ///   
        /// <exception cref="T:System.InvalidOperationException">The <paramref name="task"/> was already executed.</exception>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }

        #endregion // TryExecuteTaskInline

        #region GetScheduledTasks

        /// <summary>
        /// Generates an enumerable of <see cref="T:System.Threading.Tasks.Task"/> instances currently queued to the scheduler waiting to be executed.
        /// </summary>
        /// <returns>
        /// An enumerable that allows traversal of tasks currently queued to this scheduler.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">This scheduler is unable to generate a list of queued tasks at this time.</exception>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _requests.ToArray();
        }

        #endregion // GetScheduledTasks

        #region Dispose

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _cancellation.Cancel();
        }

        #endregion // Dispose
    }
}
