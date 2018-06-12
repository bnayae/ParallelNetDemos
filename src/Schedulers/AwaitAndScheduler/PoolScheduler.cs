using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AwaitAndScheduler
{
    public class PoolScheduler : TaskScheduler
    {
        private readonly BlockingCollection<Task> _tasks = new BlockingCollection<Task>();
        public PoolScheduler(int poolSize)
        {
            for (int i = 0; i < poolSize; i++)
            {
                var t = new Thread(() =>
                {
                    foreach (var task in _tasks.GetConsumingEnumerable())
                    {
                        base.TryExecuteTask(task);
                    }
                });
                t.Name = "Custom Pool " + i;
                t.Start();
            }
        }
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _tasks;
        }

        protected override void QueueTask(Task task)
        {
            _tasks.Add(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }
    }
}
