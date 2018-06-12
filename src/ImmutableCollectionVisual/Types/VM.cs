#region Using

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

#endregion // Using

namespace Sela.Samples
{
    public class VM<T> : INotifyPropertyChanged
    {
        #region Ctor

        public VM()
        {
            Empty();
        }

        #endregion // Ctor

        #region Commands

        #region Empty

        public void Empty()
        {
            var immutable = ImmutableList<int>.Empty;
            Data = new RootWrapper<int>(immutable, "Empty");
        }

        #endregion // Empty

        #region Add

        public void Add(int i)
        {
            Data = RootWrapper.Add(Data, i);
            Value += 1;
        }

        #endregion // Add

        #region Insert

        public void Insert(int index, int value)
        {
            Data = RootWrapper.Insert(Data, index, value);
            Value += 1;
        }

        #endregion // Insert

        #region AddRange

        public void AddRange(int start, int count)
        {
            Data = RootWrapper.AddRange(Data, Enumerable.Range(start, count).ToArray());
            Value += count;
        }

        #endregion // AddRange

        #region Remove

        public void Remove(int i)
        {
            Data = RootWrapper.Remove(Data, i);
        }

        #endregion // Remove

        #region RemoveAt

        public void RemoveAt(int index)
        {
            Data = RootWrapper.RemoveAt(Data, index);
        }

        #endregion // RemoveAt

        #endregion // Commands

        #region Value

        private int _value = 1;

        public int Value
        {
            get { return _value; }
            set 
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        #endregion // Value

        #region Data

        private RootWrapper<int> _data;
        public RootWrapper<int> Data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged();
            }
        }

        #endregion // Data

        #region PropertyChanged

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

        #endregion // PropertyChanged
    }
}
