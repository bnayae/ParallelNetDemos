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
    public interface IBinaryTree<T> : IEnumerable<IBinaryTree<T>>
    {
        IBinaryTree<T> Left { get; }
        IBinaryTree<T> Right { get; }
        bool IsFrozen { get; }
        int Height { get; }
        int Count { get; }
        T Value { get; }
        bool HasChanged { get; }
        int X { get; }
        int Y { get; }
    }
}
