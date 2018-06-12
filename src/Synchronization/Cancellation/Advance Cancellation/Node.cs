using System;
using System.Collections.Generic;
namespace Tpl.Samples
{
    public class Node
    {
        private static readonly Random _rnd = new Random(42);
        private const int CHILD_COUNT_LIMIT = 3;
        private List<Node> _childNodes = new List<Node>();

        #region Ctor

        public Node(int value)
        {
            Value = value;
        }

        #endregion // Ctor

        public int Value { get; private set; }
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
    }
}
