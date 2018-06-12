#region Using

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;

#endregion // Using

namespace Tpl.Samples
{
    /// <summary>
    /// Each node can have have up to 4 children
    /// </summary>
    public class Node
    {
        private static readonly Random _rnd = new Random(42);
        private const int CHILD_COUNT_LIMIT = 4;
        private List<Node> _childNodes = new List<Node>();
        private int _value;

        #region Ctor

        public Node(int value)
        {
            _value = value;
        }

        #endregion // Ctor

        public Node[] ChildNodes { get { return _childNodes.ToArray(); } }

        #region Add

        public void Add (int value)
        {
            if (_childNodes.Count == CHILD_COUNT_LIMIT) // when reach the limit
            {
                int index = _rnd.Next(CHILD_COUNT_LIMIT);
                _childNodes[index].Add(value); // route the call to random child
            }
            else
                _childNodes.Add(new Node(value));
        }

        #endregion // Add

        #region GetValue

        public int GetValue()
        {
            SpinWork();
            return _value;
        }

        #endregion // GetValue

        #region SpinWork

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public void SpinWork()
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < Program.SPIN_MILLISECONDS) ; // spin for the duration
            sw.Stop();
        }

        #endregion // SpinWork

    }
}
