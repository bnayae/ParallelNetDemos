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
    public class ForEachDefault: StrategyPluginBase
    {
        #region Name

        public override string Name
        {
            get { return "Parallel.ForEach"; }
        }

        #endregion // Name

        #region Description

        public override string Description
        {
            get { return @"Execute Parallel.ForEach
Array and IList<T> execution is using range partitioning
while Enumerable is using chunk partitioning.
";
            }
        }

        #endregion // Description

        #region CodeSample

        public override string CodeSample
        {
            get { return
@"Parallel.ForEach(ViewModel.SelectedDataCommand.DataCollection, tuple => ViewModel.ExecuteWorkUnit(tuple));";
            }
        }

        #endregion // CodeSample

        #region Order

        public override double Order
        {
            get { return 1.1; }
        }

        #endregion // Order

        #region Background

        public override Color Background
        {
            get { return Colors.DarkGreen; }
        }

        #endregion // Background

        protected override void OnExecute()
        {
            Parallel.ForEach(ViewModel.SelectedDataCommand.DataCollection, tuple => ViewModel.ExecuteWorkUnit(tuple));
        }
    }
}
