using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CompareAndSwapStack
{
    public class FreeNode<T>
    {
        #region Ctor

        public FreeNode(T value)
        {
            Value = value;
        }

        #endregion // Ctor

        public T Value { get; private set; }

        #region Next

        private FreeNode<T> _next;
        public FreeNode<T> Next 
        {
            get
            {
                return _next;
            }
            set
            {
                FreeNode<T> oldNextResult =
                     Interlocked.CompareExchange (ref _next, value, null);
                if (oldNextResult != null)
                    oldNextResult.Next = value;
            }
        }

        #endregion // Next
    }
}
