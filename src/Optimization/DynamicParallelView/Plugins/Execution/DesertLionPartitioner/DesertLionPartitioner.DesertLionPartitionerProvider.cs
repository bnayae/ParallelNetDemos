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
        private partial class DesertLionPartitionerProvider<T> : CustomOrderablePartitioner<T>
        {
            #region Constructors

            public DesertLionPartitionerProvider(IEnumerable<T> data)
                : base(data)
            { }

            #endregion // Constructors

            #region GetOrderableDynamicPartitions

            /// <summary>
            /// Get partition provider
            /// </summary>
            /// <returns></returns>
            public override IEnumerable<KeyValuePair<long, T>> GetOrderableDynamicPartitions()
            {
                return new DesertLionEnumerable(_data);
            }

            #endregion // GetOrderableDynamicPartitions
        }
    }

}
