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

#endregion // Using

namespace Tpl.Samples
{
    /// <summary>
    /// General implementation of orderable partition
    /// which get the partitioning strategy (Func<IEnumerable<KeyValuePair<long, T>>>) as constructor's parameter 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CustomOrderablePartitioner<T> : OrderablePartitioner<T>
    {
        #region Private / Protected Fields

        protected IList<T> _data;

        #endregion // Private / Protected Fields

        #region Constructors

        public CustomOrderablePartitioner(IEnumerable<T> data)
            : base(true, false, true)
        {
            _data = data as IList<T>;
            if (_data == null)
                _data = new List<T>(data);
        }

        #endregion // Constructors

        #region SupportsDynamicPartitions

        public override bool SupportsDynamicPartitions
        {
            get
            {
                return true; // must be true (it belong to the Partitioner base class)
            }
        }

        #endregion // SupportsDynamicPartitions

        #region GetOrderablePartitions

        /// <summary>
        /// gets list of enumerators
        /// </summary>
        /// <param name="partitionCount"></param>
        /// <returns></returns>
        public override IList<IEnumerator<KeyValuePair<long, T>>> GetOrderablePartitions(int partitionCount)
        {
            var dynamicPartitionProviders = GetOrderableDynamicPartitions();

            // array of enumerators (each enumerator item is a pair of the item index and the item value)
            var enumerators = new IEnumerator<KeyValuePair<long, T>>[partitionCount];
            for (int i = 0; i < partitionCount; i++) // get enumerator per partition (core)
            {
                enumerators[i] = dynamicPartitionProviders.GetEnumerator();
            }
            return enumerators;
        }

        #endregion // GetOrderablePartitions
    }

}
