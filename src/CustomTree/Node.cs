using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomTree
{
    public class Node: IEnumerable<int>
    {
        public Node(int value)
        {
            Value = value;
        }
        public int Value { get; private set; }

        public void Add(int value)
        {
            Node node = new Node(value);
            Add(node);
        }
        private void Add(Node node)
        {
            if (Value < node.Value)
            {
                Node right = Interlocked.CompareExchange(ref _right, node, null);
                if (right != null)
                    right.Add(node);
            }
            else
            {
                Node left = Interlocked.CompareExchange(ref _left, node, null);
                if (left != null)
                    left.Add(node);
            }
        }

        private Node _left;

        public Node Left
        {
            get { return _left; }
        }

        private Node _right;

        public Node Right
        {
            get { return _right; }
        }

        public IEnumerator<int> GetEnumerator()
        {
            if (Left != null)
            {
                foreach (var item in Left)
                {
                    yield return item;
                }
            }

            yield return Value;

            if (Right != null)
            {
                foreach (var item in Right)
                {
                    yield return item;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
