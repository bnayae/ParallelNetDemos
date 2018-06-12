using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Sela.Samples
{
    public class CustomScheduler : TaskScheduler
    {
        private readonly Thread[] _singleExecutor = new Thread[1];
        private readonly BlockingCollection<Task> _tasks = new BlockingCollection<Task>();

        public CustomScheduler()
        {
            for (int i = 0; i < _singleExecutor.Length; i++)
            {
                var t = new Thread(() =>
                  {
                      foreach (var task in _tasks.GetConsumingEnumerable())
                      {
                          base.TryExecuteTask(task);
                      }
                  });
                t.Name = "Custom";
                t.Start();
                _singleExecutor[i] = t;
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
