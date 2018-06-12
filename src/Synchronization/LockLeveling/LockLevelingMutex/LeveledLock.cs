#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

#endregion // Using

namespace Tpl.Samples
{
    public class LeveledLock : IDisposable
    {
        private static readonly object _localSync = new object();
        private static int _currentGlobalLevel = int.MaxValue; // last lock index
        private readonly object _syncInstanceLevel;

        private static IDictionary<object, LeveledLock> _locks = // Acquired locks state
            new Dictionary<object, LeveledLock>(); 

        private LeveledLock(object sync)
        {
            _syncInstanceLevel = sync;
            RefCount = 1;
        }

        public static LeveledLock Acquire(object sync)
        {
            LeveledLock leveledLock;
            lock (_localSync)
            {
                bool isAdded;
                if (_locks.ContainsKey(sync))
                {
                    leveledLock = _locks[sync];
                    leveledLock.RefCount += 1; // used for disposal
                    isAdded = false;
                }
                else
                {
                    leveledLock = new LeveledLock(sync);
                    _locks.Add(sync, leveledLock); // set the lock leveling
                    isAdded = true;
                }

                if (leveledLock.Level > _currentGlobalLevel) // check for potential deadlock
                    throw new ThreadStateException("Potential deadlock");

                if (isAdded) // if the 
                {
                    _currentGlobalLevel--;
                    leveledLock.Level = _currentGlobalLevel;
                }
            }

            Monitor.Enter(sync);

            return leveledLock;
        }

        public int Level { get; private set; }

        public int RefCount { get; private set; }

        public void Dispose() // dismiss lock
        {
            Monitor.Exit(_syncInstanceLevel);
            RefCount--;
            if (RefCount == 0)
            {
                lock (_localSync)
                {
                    if (RefCount == 0)
                        this.Level = -1;
                }
            }
        }
    }
}
