#region Using

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#endregion // Using

namespace Sela.Samples
{
    [DebuggerDisplay("{Value}: Location = ({X},{Y}), Height = {Height}, IsFrozen = {IsFrozen}, Count = {Count}")]
    public class BinaryTree<T> : IBinaryTree<T>, IEnumerable<IBinaryTree<T>>
    {
        private readonly IEnumerable<IBinaryTree<T>> _emptyEnumerable = Enumerable.Empty<IBinaryTree<T>>();

        public IBinaryTree<T> Left { get; set; }
        public IBinaryTree<T> Right { get; set; }
        public T Value { get; set; }
        public bool IsFrozen { get; set; }
        public int Height { get; set; }
        public int Count { get; set; }
        public bool HasChanged { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        #region IEnumerable members

        public IEnumerator<IBinaryTree<T>> GetEnumerator()
        {
            foreach (var item in (Left ?? _emptyEnumerable))
            {
                yield return item;
            }
            yield return this;
            foreach (var item in Right ?? _emptyEnumerable)
            {
                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion // IEnumerable members
    }
}
