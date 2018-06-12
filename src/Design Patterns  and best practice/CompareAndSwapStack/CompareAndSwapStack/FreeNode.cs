using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CompareAndSwapStack
{
    public class FreeNode<T>
    {
        public FreeNode(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }

        private FreeNode<T> _next;
        public FreeNode<T> Next 
        {
            get
            {
                return _next;
            }
            set // lock-free assignment
            {
                FreeNode<T> oldNextResult =
                     Interlocked.CompareExchange (ref _next, value, null);
                if (oldNextResult != null) // _next was assigned by other thread
                    oldNextResult.Next = value;
            }
        }
    }
}
