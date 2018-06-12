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
    public class FirstSlowList : DataPluginBase
    {
        #region Name

        public override string Name
        {
            get { return "First Slow IList<T>"; }
        }

        #endregion // Name

        #region Description

        public override string Description
        {
            get { return @"IList<T>: first processing each work unit is slow"; }
        }

        #endregion // Description

        #region Order

        public override double Order
        {
            get { return 4; }
        }

        #endregion // Order

        public override IEnumerable<DataItem> DataCollection
        {
            get
            {
                var data = Enumerable.Range(0, ViewModel.ItemsPerCore * Environment.ProcessorCount);
                return data.Select(item =>
                {
                    DataItem unit;
                    if (item < 5 * Environment.ProcessorCount)
                        unit = new DataItem(item, 6);
                    else
                        unit = new DataItem(item, 2);
                    return unit;
                }).ToList();
            }
        }
    }
}
