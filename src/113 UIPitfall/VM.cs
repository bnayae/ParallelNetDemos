using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sela.Samples
{
    public class VM: INotifyPropertyChanged, ICommand
    {
        #region Data

        private int _data;

        public int Data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged();
            }
        }

        #endregion // Data

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion // INotifyPropertyChanged Members

        #region CanExecute

        public bool CanExecute(object parameter)
        {
            return true;
        }

        #endregion // CanExecute

        #region CanExecuteChanged

        public event EventHandler CanExecuteChanged;

        #endregion // CanExecuteChanged

        public void Execute(object parameter)
        {
            Data = 0;
            if (parameter.Equals("R"))
            {
                Data = Service.ExecuteAsync().Result;
            }
            else
            {
                Task t = ExecuteAsync();
            }
        }

        public async Task ExecuteAsync()
        {
            Data = await Service.ExecuteAsync();
        }
    }
}
