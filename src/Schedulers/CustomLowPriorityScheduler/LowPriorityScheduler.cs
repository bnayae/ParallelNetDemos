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
    /// Custom task scheduler that execute the task on thread under specific priority
    /// </summary>
    /// <remarks>
    /// No partitioning optimization included
    /// </remarks>
    #endregion // Documentation
    public class LowPriorityScheduler : TaskScheduler
    {
        #region Private / Protected Fields

        private const string DEFAULT_NAME = "Custom Priority Scheduler";
        // low priority thread
        private readonly Thread _workerThread;
        // the threads will iterate on the collection to consume scheduled task
        // when no task scheduled, the threads will be block by the collection
        private BlockingCollection<Task> _collection = new BlockingCollection<Task>();

        #endregion // Private / Protected Fields

        #region Ctor

        public LowPriorityScheduler()
        {
            _workerThread = new Thread(WorkConsumer);
            _workerThread.Name = string.Format("Low Priority Scheduler");
            _workerThread.IsBackground = true;
            _workerThread.Priority = ThreadPriority.BelowNormal;
            _workerThread.Start();
        }

        #endregion // Ctor

        #region WorkConsumer

        /// <summary>
        /// consume scheduled work units on thread with specific priority
        /// </summary>
        private void WorkConsumer()
        {
            foreach (Task task in _collection.GetConsumingEnumerable())
            {
                // execute the task under the current thread
                if (!base.TryExecuteTask(task))
                    Trace.WriteLine("Error");
            }
        }

        #endregion // WorkConsumer

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
            return _collection;
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
            _collection.Add(task); // add task to the blocking collection (produce work)
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
    }
}
