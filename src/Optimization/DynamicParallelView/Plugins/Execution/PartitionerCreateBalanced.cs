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
    public class PartitionerCreateBalanced : StrategyPluginBase
    {
        #region Name

        public override string Name
        {
            get { return "Partitioner (chunk/balanced)"; }
        }

        #endregion // Name

        #region Description

        public override string Description
        {
            get
            {
                return @"Execute Parallel.ForEach of custom partitioner with load balance (chunk).
";
            }
        }

        #endregion // Description

        #region CodeSample

        public override string CodeSample
        {
            get { return
@"var data = ViewModel.SelectedDataCommand.DataCollection;
var parallelQuery = Partitioner.Create(data, true);
Parallel.ForEach(parallelQuery, tuple => ViewModel.ExecuteWorkUnit(tuple));
";
            }
        }

        #endregion // CodeSample

        #region Order

        public override double Order
        {
            get { return 3.2; }
        }

        #endregion // Order

        #region Background

        public override Color Background
        {
            get { return Colors.DarkViolet; }
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
            var parallelQuery = Partitioner.Create(data.ToList(), true);
            Parallel.ForEach(parallelQuery, tuple => ViewModel.ExecuteWorkUnit(tuple));
        }
    }
}
