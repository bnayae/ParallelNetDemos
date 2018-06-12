#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;

#endregion // Using

namespace Tpl.Samples
{
    #region Documentation
    /// <summary>
    /// Custom task scheduler that log execution time
    /// </summary>
    #endregion // Documentation
    public class CustomLogScheduler: TaskScheduler, IEnumerable<TimeSpan>
    {
        private TaskScheduler _scheduler;
        private ConcurrentQueue<TimeSpan> _timeing = new ConcurrentQueue<TimeSpan>(); // execution time list
        private Lazy<Stopwatch> _watch = new Lazy<Stopwatch>(Stopwatch.StartNew);

        public CustomLogScheduler(TaskScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        #region GetScheduledTasks

        #region Documentation
        /// <summary>
        /// Generates an enumerable of System.Threading.Tasks.Task instances currently
        /// queued to the scheduler waiting to be executed.
        /// </summary>
        /// <returns>
        /// An enumerable that allows traversal of tasks currently queued to this scheduler.
        /// </returns>
        #endregion // Documentation
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            throw new NotImplementedException();
        }

        #endregion // GetScheduledTasks

        #region QueueTask

        #region Documentation
        /// <summary>
        /// Queues a System.Threading.Tasks.Task to the scheduler. 
        /// </summary>
        /// <param name="task">
        /// The System.Threading.Tasks.Task to be queued.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The task argument is null.
        /// </exception>
        #endregion // Documentation
        protected override void QueueTask(Task task)
        {
            var c = new CancellationTokenSource();
            Task.Factory.StartNew(state =>
                {
                    base.TryExecuteTask(state as Task);
                    _timeing.Enqueue(_watch.Value.Elapsed);
                }, task, c.Token, TaskCreationOptions.None, _scheduler);        
        }

        #endregion // QueueTask

        #region TryExecuteTaskInline

        #region Documentation
        /// <summary>
        /// Determines whether the provided System.Threading.Tasks.Task can be executed
        /// synchronously in this call, and if it can, executes it.
        /// </summary>
        /// <param name="task">
        /// The System.Threading.Tasks.Task to be executed.
        /// </param>
        /// <param name="taskWasPreviouslyQueued">
        /// taskWasPreviouslyQueued:
        ///   A Boolean denoting whether or not task has previously been queued. If this
        ///   parameter is True, then the task may have been previously queued (scheduled);
        ///   if False, then the task is known not to have been queued, and this call is
        ///   being made in order to execute the task inline without queuing it.
        /// </param>
        /// <returns>
        /// A Boolean value indicating whether the task was executed inline.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// The task argument is null
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// The task was already executed.
        /// </exception>
        #endregion // Documentation
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false; // we might not want to execute task that should schedule as high or low priority inline
            //return base.TryExecuteTask(task);
        }

        #endregion // TryExecuteTaskInline

        #region IEnumerable<TimeSpan> Members

        public IEnumerator<TimeSpan> GetEnumerator()
        {
            return _timeing.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion // IEnumerable<TimeSpan> Members
    }
}
