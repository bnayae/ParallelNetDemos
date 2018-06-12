#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Media;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#endregion // Using

namespace Tpl.Samples
{
    public class ParallelWorkUnitTrace
    {
        internal const int WIDTH = 50;
        internal const int LEFT_SPACE = 5;

        #region Private / Protected Fields

        private static readonly Color[] THREAD_COLORS = new[] { Colors.Green, Colors.Blue, Colors.IndianRed, Colors.Purple, Colors.Maroon, Colors.Chartreuse, Colors.Yellow, Colors.DarkRed, Colors.DeepPink, Colors.OrangeRed };
        private static long _floor = 0;
        private IProducerConsumerCollection<ParallelWorkUnitTrace> _collection;
        private Stopwatch _watch;
        private long _startAt;
        private long _endAt;
        private DataItem _data = null;
        private bool _hasSwichCore = false;
        private Func<ArrangeBy> _arrangeStrategy;
        private double _heightFactor = 10;

        #endregion // Private / Protected Fields

        #region Floor

        public static long Floor
        {
            get { return _floor; }
            set { _floor = value; }
        }

        #endregion // Floor

        #region Init

        [Obsolete("Blendbility", true)]
        public ParallelWorkUnitTrace()
        {
        }

        private ParallelWorkUnitTrace(
            DataItem data,
            double heightFactor,
            Stopwatch watch,
            IProducerConsumerCollection<ParallelWorkUnitTrace> collection,
            Func<ArrangeBy> arrangeStrategy)
        {
            _heightFactor = heightFactor;
            _startAt = watch.ElapsedMilliseconds;
            CoreId = GetCurrentProcessorNumber();
            var threadId = Thread.CurrentThread.ManagedThreadId;
            ThreadId = threadId;
            _watch = watch;

            _data = data;
            _collection = collection;
            _arrangeStrategy = arrangeStrategy;
            IsParent = true;
        }

        public static ParallelWorkUnitTrace Create(
            DataItem data,
            double heightFactor,
            Stopwatch watch,
            IProducerConsumerCollection<ParallelWorkUnitTrace> collection,
            Func<ArrangeBy> arrangeStrategy)
        {
            var instance = new ParallelWorkUnitTrace(data, heightFactor, watch, collection, arrangeStrategy);
            instance.SpinWork(data.SpinDuration);
            instance.Close();
            collection.TryAdd(instance);
            return instance;
        }

        #endregion // Init

        #region Properties

        #region CoreId

        public int CoreId { get; private set; }

        #endregion // CoreId

        #region ThreadId

        public int ThreadId { get; private set; }

        #endregion // ThreadId

        #region ThreadIndex

        public int ThreadIndex { get; set; }

        #endregion // ThreadIndex

        #region Data

        public object Data
        {
            get
            {
                if (_arrangeStrategy() == ArrangeBy.Thread)
                    return IsParent ? _data : null;
                return _data;
            }
        }

        #endregion // Data

        #region CoreThreadId

        public int CoreThreadId
        {
            get
            {
                if (_arrangeStrategy() == ArrangeBy.Thread)
                    return CoreId;
                else
                    return ThreadId;
            }
        }

        #endregion // CoreThreadId

        #region Left

        public int Left
        {
            get
            {
                int value = _arrangeStrategy() == ArrangeBy.Core ? CoreId : ThreadIndex;
                return value * (WIDTH + LEFT_SPACE);
            }
        }

        #endregion // Left

        #region StartAt

        public long StartAt
        {
            get
            {
                return _startAt;
            }
        }

        #endregion // StartAt

        #region Top

        public long Top
        {
            get
            {
                return Convert.ToInt32((_startAt - _floor) * _heightFactor);
            }
        }

        #endregion // Top

        #region Width

        public long Width
        {
            get { return WIDTH - 2; }
        }

        #endregion // Width

        #region Height

        public long Height
        {
            get
            {
                long endAt = _endAt;
                if (_arrangeStrategy() == ArrangeBy.Thread && IsParent)
                {
                    ParallelWorkUnitTrace unit = this;
                    while (unit.Child != null)
                    {
                        unit = unit.Child;
                        endAt = unit._endAt;
                    }
                }
                return Convert.ToInt32((endAt - _startAt) * _heightFactor);
            }
        }

        #endregion // Height

        #region BackColor

        public Color BackColor
        {
            get
            {
                Color c;
                if (ThreadIndex < THREAD_COLORS.Length)
                    c = THREAD_COLORS[ThreadIndex];
                else
                    c = Color.FromArgb(255, (byte)(ThreadIndex % 3 * 50), (byte)((ThreadIndex + 1) % 3 * 50), (byte)((ThreadIndex + 2) % 3 * 50));

                return c;
            }
        }

        #endregion // BackColor

        #region HasSwichCore

        public bool HasSwichCore
        {
            get
            {
                if (_arrangeStrategy() == ArrangeBy.Thread)
                    return IsParent ? _hasSwichCore : false;
                return _hasSwichCore;
            }
        }

        #endregion // HasSwichCore

        #region IsParent

        public bool IsParent { get; private set; }

        #endregion // IsParent

        #region Level

        private int Level
        {
            get;
            set;
        }

        #endregion // Level

        #region Child

        public ParallelWorkUnitTrace Child { get; private set; }

        #endregion // Child

        #endregion // Properties

        #region GetCurrentProcessorNumber

        [DllImport("Kernel32.dll"), SuppressUnmanagedCodeSecurity]
        public static extern int GetCurrentProcessorNumber();

        #endregion // GetCurrentProcessorNumber

        #region SpinWork

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SpinWork(long milliseconds)
        {
            int i = 0;
            int checkFactor = milliseconds < 50 ? 10 : 1000;
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < milliseconds) // spin for the duration
            {
                i++;
                if (i % checkFactor == 0 && this.Level < 20)
                {
                    int coreId = GetCurrentProcessorNumber();
                    if (coreId != this.CoreId)
                    {
                        sw.Stop();
                        _hasSwichCore = true;
                        Interlocked.CompareExchange(ref _endAt, _watch.ElapsedMilliseconds, 0);
                        var spinDuration = _data.SpinDuration - sw.ElapsedMilliseconds;
                        var unit = ParallelWorkUnitTrace.Create(_data, _heightFactor, _watch, _collection, _arrangeStrategy); // switch core
                        unit._hasSwichCore = true;
                        Child = unit;
                        unit.Level = this.Level + 1;
                        unit.IsParent = false;
                        break;
                    }
                }
            }
            sw.Stop();
        }

        #endregion // SpinWork

        #region Close

        public void Close()
        {
            Interlocked.CompareExchange(ref _endAt, _watch.ElapsedMilliseconds, 0);
            _watch = null;
        }

        #endregion // Close
    }
}
