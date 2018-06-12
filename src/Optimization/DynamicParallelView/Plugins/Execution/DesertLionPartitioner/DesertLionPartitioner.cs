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
    public partial class DesertLionPartitioner : StrategyPluginBase
    {
        #region Name

        public override string Name
        {
            get { return "Desert Lion Partitioner"; }
        }

        #endregion // Name

        #region Description

        public override string Description
        {
            get
            {
                return @"Execute Parallel.ForEach of custom orderable partitioner 
while work units execution take is equals for all work units.
the partitioning is using Desert Lion strategy.
";
            }
        }

        #endregion // Description

        #region CodeSample

        public override string CodeSample
        {
            get
            {
                return
@"var data = ViewModel.SelectedDataCommand.DataCollection;
var parallelQuery = new DesertLionPartitionerProvider<DataItem>(data);
Parallel.ForEach(parallelQuery, tuple => ViewModel.ExecuteWorkUnit(tuple));
";
            }
        }

        #endregion // CodeSample

        #region Order

        public override double Order
        {
            get { return 4; }
        }

        #endregion // Order

        #region Background

        public override Color Background
        {
            get { return Colors.IndianRed; }
        }

        #endregion // Background

        #region CanExecute

        public override bool CanExecute(object parameter)
        {
            if (ViewModel == null || ViewModel.SelectedDataCommand == null)
                return false;
            var data = ViewModel.SelectedDataCommand.DataCollection;
            return data is IList<DataItem>;
        }

        #endregion // CanExecute

        protected override void OnExecute()
        {
            var data = ViewModel.SelectedDataCommand.DataCollection;
            var parallelQuery = new DesertLionPartitionerProvider<DataItem>(data);
            Parallel.ForEach(parallelQuery, tuple => ViewModel.ExecuteWorkUnit(tuple));
        }
    }

}
