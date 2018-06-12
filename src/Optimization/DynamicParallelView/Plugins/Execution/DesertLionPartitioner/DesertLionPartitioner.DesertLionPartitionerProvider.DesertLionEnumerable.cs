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
using System.Windows.Input;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Collections;

#endregion // Using

namespace Tpl.Samples
{
    partial class DesertLionPartitioner 
    {
        partial class DesertLionPartitionerProvider<T> 
        {
            #region Documentation
            /// <summary>
            /// break list into dynamically using Desert Lion partitions strategy
            /// the responsibility of this type is to break list into into sub list partitions
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <remarks>
            /// GetEnumerator will be invoked once for each degree of parallelism  (core by default)
            /// each enumerator item is a pair of the item index and the item value
            /// </remarks>
            #endregion // Documentation
            internal class DesertLionEnumerable
                : IEnumerable<KeyValuePair<long, T>>
            {
                #region Private / Protected Fields

                private int _nextStartIndex = 0;
                private IList<T> _data;

                #endregion // Private / Protected Fields

                #region Constructors

                public DesertLionEnumerable(IList<T> data)
                {
                    _data = data;
                }

                #endregion // Constructors

                #region GetEnumerator

                /// <summary>
                /// return partition on demand
                /// this method will be execute once for each degree of parallelism 
                /// (the default is once per core)
                /// </summary>
                /// <returns></returns>
                /// <remarks>thread safe</remarks>
                public IEnumerator<KeyValuePair<long, T>> GetEnumerator()
                {
                    while (true)
                    {
                        int partitionSize;
                        int startAt; // the start index of the current thread partition enumerator
                        lock (this) // set the next partition slice (thread-safe)
                        {
                            startAt = _nextStartIndex;
                            partitionSize = (_data.Count - startAt) / (Environment.ProcessorCount * 2);
                            partitionSize = partitionSize < 1 ? 1 : partitionSize;
                            _nextStartIndex += partitionSize;
                        }

                        for (int i = startAt; i < startAt + partitionSize; i++) // Iterate over the current partition
                        {
                            if (i >= _data.Count)
                                yield break; // no more items to execute (exit term)

                            yield return new KeyValuePair<long, T>(
                                 i, _data[i]);
                        }
                    }
                }

                #region Overloads

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return ((IEnumerable<KeyValuePair<long, T>>)this).GetEnumerator();
                }

                #endregion // Overloads

                #endregion // GetEnumerator
            }
        }
    }

}
