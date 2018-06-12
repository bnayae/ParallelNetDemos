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
    public class CustomPriorityScheduler: TaskScheduler
    {
        #region Private / Protected Fields

        private const string DEFAULT_NAME = "Custom Priority Scheduler";
        // Array of live threads with specific priority level
        private readonly Lazy<Thread>[] _workerThread; 
        // the threads will iterate on the collection to consume scheduled task
        // when no task scheduled, the threads will be block by the collection
        private BlockingCollection<Task> _collection = new BlockingCollection<Task>();
        private readonly int _maximumConcurrencyLevel;

        #endregion // Private / Protected Fields

        #region Ctor

        public CustomPriorityScheduler(ThreadPriority priority, string name = DEFAULT_NAME, int maximumConcurrencyLevel = -1)
        {
            _maximumConcurrencyLevel = maximumConcurrencyLevel < 0 ? Environment.ProcessorCount : maximumConcurrencyLevel;
            _workerThread = new Lazy<Thread>[_maximumConcurrencyLevel];
            for (int i = 0; i < _maximumConcurrencyLevel; i++)
            {
                int local = i;
                _workerThread[i] = new Lazy<Thread>(() => // define the thread information
                {
                    var trd = new Thread(WorkConsumer);
                    int extension = (name == DEFAULT_NAME) ? this.GetHashCode() : local;
                    trd.Name = string.Format("{0}: {1}", name, extension);
                    trd.IsBackground = true;
                    trd.Priority = priority;
                    return trd;
                });
            }
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
                base.TryExecuteTask(task);
            }
        }

        #endregion // WorkConsumer

        #region MaximumConcurrencyLevel

        #region Documentation
        /// <summary>
        /// Indicates the maximum concurrency level this 
        /// System.Threading.Tasks.TaskScheduler is able to support.
        /// </summary>
        #endregion // Documentation
        public override int MaximumConcurrencyLevel
        {
            get
            {
                return _maximumConcurrencyLevel;
            }
        }

        #endregion // MaximumConcurrencyLevel

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
            _collection.Add (task); // add task to the blocking collection (produce work)

            if (!_workerThread[0].IsValueCreated)
            {
                for (int i = 0; i < _maximumConcurrencyLevel; i++)
                {
                    _workerThread[i].Value.Start(); // see WorkConsumer
                }
            }
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
