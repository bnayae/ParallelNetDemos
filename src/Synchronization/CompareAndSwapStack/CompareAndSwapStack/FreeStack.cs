using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;

namespace CompareAndSwapStack
{
    public class FreeStack<T>: IEnumerable<T>
    {
        #region Root

        private FreeNode<T> _root;
        public FreeNode<T> Root 
        {
            get { return _root; }
            private set { _root = value; }
        }

        #endregion // Root

        #region Push

        public void Push(T value)
        {
            var node = new FreeNode<T>(value);
            FreeNode<T> oldRootResult =
                Interlocked.CompareExchange(ref _root, node, null);
            if (oldRootResult != null)
                oldRootResult.Next = node;
        }

        #endregion // Push

        #region Pop

        public FreeNode<T> Pop()
        {
            FreeNode<T> oldRootResult = null;
            FreeNode<T> oldRoot;
            do
            {
                oldRoot = _root;
                if (oldRoot == null)
                    break;
                oldRootResult = Interlocked.CompareExchange(ref _root, oldRoot.Next, oldRoot);
            } while (oldRootResult != oldRoot);

            return oldRootResult;
        }

        #endregion // Pop

        #region IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            FreeNode<T> node = _root;
            while (node != null)
            {
                yield return node.Value;
                node = node.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<T>).GetEnumerator();
        }

        #endregion // IEnumerable
    }
}
