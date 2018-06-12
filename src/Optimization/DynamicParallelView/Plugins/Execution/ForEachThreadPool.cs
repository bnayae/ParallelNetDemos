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
    public class ForEachThreadPool: StrategyPluginBase
    {
        #region Name

        public override string Name
        {
            get { return "ForEach ThreadPool"; }
        }

        #endregion // Name

        #region Description

        public override string Description
        {
            get
            {
                return @"Execute foreach with open threadPool";
            }
        }

        #endregion // Description

        #region CodeSample

        public override string CodeSample
        {
            get { return
@"var data = ViewModel.SelectedDataCommand.DataCollection;
var counddown = new CountdownEvent(data.Count());
foreach (var tuple in data)
{
    ThreadPool.QueueUserWorkItem ((state) =>
        {
            ViewModel.ExecuteWorkUnit(state as DataItem);
            counddown.Signal();
        }, tuple);
}
counddown.Wait();
";
            }
        }

        #endregion // CodeSample

        #region Order

        public override double Order
        {
            get { return 0.1; }
        }

        #endregion // Order

        #region Background

        public override Color Background
        {
            get { return Colors.DarkOrange; }
        }

        #endregion // Background

        protected override void OnExecute()
        {
            var data = ViewModel.SelectedDataCommand.DataCollection;
            var counddown = new CountdownEvent(data.Count());
            foreach (var tuple in data)
            {
                ThreadPool.QueueUserWorkItem ((state) =>
                    {
                        ViewModel.ExecuteWorkUnit(state as DataItem);
                        counddown.Signal();
                    }, tuple);
            }
            counddown.Wait();
        }
    }
}
