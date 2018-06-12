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
    public abstract class StrategyPluginBase : IStrategyPlugin, ICommand, IPartImportsSatisfiedNotification
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string CodeSample { get; }
        public abstract double Order { get; }
        public abstract Color Background { get; }

        [Import]
        protected IViewModel ViewModel { get; private set; }
        [ImportMany]
        private IDataPlugin[] DataProviders { get; set; }

        protected virtual double HeightFactor { get { return ViewModel.SelectedDataCommand.HeightFactor; } }

        #region ICommand members

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged = (s, e) => { };

        public void Execute(object parameter)
        {
            ViewModel.Start(OnExecute);
        }

        #endregion // ICommand members

        public ICommand Command
        {
            get { return this; }
        }

        protected abstract void OnExecute();

        public void OnImportsSatisfied()
        {
            ViewModel.DataSelectionChanged += (s, e) => CanExecuteChanged(s, e);
        }
    }
}
