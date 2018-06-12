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
    public class EqualsEnumerable : DataPluginBase
    {
        #region Name

        public override string Name
        {
            get { return "Equals Enumerable"; }
        }

        #endregion // Name

        #region Description

        public override string Description
        {
            get { return @"Enumerable: processing each work unit is equals";}
        }

        #endregion // Description

        #region Order

        public override double Order
        {
            get { return 1; }
        }

        #endregion // Order

        public override IEnumerable<DataItem> DataCollection
        {
            get
            {
                var data = Enumerable.Range(0, ViewModel.ItemsPerCore * Environment.ProcessorCount);
                return data.Select(item => new DataItem(item, 2));
            }
        }

        //protected override IEnumerable<DataItem> OnDataFactory()
        //{
        //    var data = Enumerable.Range(0, ViewModel.ItemsPerCore * Environment.ProcessorCount);
        //    return data.Select(item => new DataItem(item, 2));
        //}
    }
}
