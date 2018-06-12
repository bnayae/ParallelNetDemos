#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Windows.Threading;

#endregion // Using

namespace Tpl.Samples
{
    [Export(typeof(IViewModel))]
    public class ParallelViewModel: ConcurrentQueue<ParallelWorkUnitTrace>, 
        IViewModel, 
        INotifyPropertyChanged, INotifyCollectionChanged,
        IPartImportsSatisfiedNotification
    {
        #region Private Fields

        private Stopwatch _watch = new Stopwatch();
        private ArrangeBy _arrangeBy = ArrangeBy.Core;
        private CompositionContainer _container;
        private bool _isProcessing = false;
        private string _currentDataStrategy;
        private IDataPlugin _selectedDataCommand;

        #endregion // Private Fields

        #region Constructors

        public ParallelViewModel()
        {
            ItemsPerCore = 20;
            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            _container = new CompositionContainer(catalog);
            _container.ComposeParts(this);
            //DataFactory = () => Enumerable.Range(0, ItemsPerCore * Environment.ProcessorCount)
            // .Select(item => new DataItem(item, 2, 20)); // default
        }

        #endregion // Constructors

        #region Property Changed Event

        public event PropertyChangedEventHandler PropertyChanged = (s, a) => { };
        public event NotifyCollectionChangedEventHandler CollectionChanged = (s, a) => { };

        #endregion // Property Changed Event

        public event EventHandler DataSelectionChanged = (s, e) => { };

        #region Public Properties

        #region Commands

        [ImportMany]
        public IStrategyPlugin[] Commands { get; private set; }

        public IEnumerable<IStrategyPlugin> CommandsOrdered { get { return Commands.OrderBy(p => p.Order); } }

        #endregion // Commands

        #region DataCommands

        [ImportMany]
        public IDataPlugin[] DataCommands { get; private set; }

        public IEnumerable<IDataPlugin> DataCommandsOrdered { get { return DataCommands.OrderBy(p => p.Order); } }

        #endregion // Commands

        #region SelectedDataCommand

        public IDataPlugin SelectedDataCommand {
            get 
            {
                return _selectedDataCommand; 
            }
            set
            {
                _selectedDataCommand = value;
                DataSelectionChanged(this, EventArgs.Empty);
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedDataCommand"));                
            }
        }

        #endregion // SelectedDataCommand

        #region Data

        public Func<IEnumerable<DataItem>> DataFactory { get; set; }

        #endregion // Data

        #region ItemsPerCore

        private int _itemsPerCore;
        public int ItemsPerCore 
        {
            get { return _itemsPerCore; }
            set
            {
                if (_itemsPerCore == value)
                    return;
                _itemsPerCore = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ItemsPerCore"));
            }
        }

        #endregion // ItemsPerCore

        #region TotalMilliseconds

        private long _totalmilliseconds;
        public long TotalMilliseconds 
        {
            get 
            {
                if (SelectedDataCommand == null)
                    return 0;
                return Convert.ToInt64(_totalmilliseconds * SelectedDataCommand.HeightFactor);
            }
            set
            {
                if (_totalmilliseconds == value)
                    return;
                _totalmilliseconds = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TotalMilliseconds"));
            }
        }

        #endregion // TotalMilliseconds

        #region CanvasWidth

        private long _threadCount;
        public long CanvasWidth 
        {
            get 
            {
                long result;
                if (ArrangeByCore)
                    result = Environment.ProcessorCount * (ParallelWorkUnitTrace.WIDTH + ParallelWorkUnitTrace.LEFT_SPACE);
                else
                    result = _threadCount * (ParallelWorkUnitTrace.WIDTH + ParallelWorkUnitTrace.LEFT_SPACE);
                
                return result;
            }
        }

        #endregion // CanvasWidth

        #region ArrangeByCore

        public bool ArrangeByCore 
        {
            get { return _arrangeBy == ArrangeBy.Core; }
            set
            {

                _arrangeBy = value?ArrangeBy.Core:ArrangeBy.Thread;
                PropertyChanged(this, new PropertyChangedEventArgs("CanvasWidth"));
                PropertyChanged(this, new PropertyChangedEventArgs("ArrangeByCore"));
                PropertyChanged(this, new PropertyChangedEventArgs("ViewTitle"));
                Refresh();
            }
        }

        #endregion // ArrangeByCore

        #region ViewTitle

        public string ViewTitle
        {
            get { return _arrangeBy.ToString() + " view"; }
        }

        #endregion // ViewTitle

        #region Visibility

        public Visibility IsProcessing
        {
            get { return _isProcessing?Visibility.Visible:Visibility.Collapsed; }
        }

        #endregion // Visibility

        #region CurrentDataStrategy

        public string CurrentDataStrategy 
        {
            get { return _currentDataStrategy; }
            set
            {
                _currentDataStrategy = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CurrentDataStrategy"));
            }
        }

        #endregion // CurrentDataStrategy

        #endregion // Public Properties

        #region Public Methods

        #region Start

        public void Start(Action action)
        {
            var dispatcherScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task t = Task.Factory.StartNew ( () =>
            {
                _isProcessing = true;
                PropertyChanged(this, new PropertyChangedEventArgs("IsProcessing"));
                
                ParallelWorkUnitTrace item;            
                while(base.TryDequeue(out item)); // clear 
                GC.Collect();
                Thread.Sleep(0);
                lock (this)
                {
                    _watch.Restart();
                    action();
                    _watch.Stop();

                    #region Modify result

                    var indexMapper = new Dictionary<int, int>();
                    int i = 0;
                    ParallelWorkUnitTrace.Floor = long.MaxValue;
                    foreach (ParallelWorkUnitTrace unit in this)
                    {
                        #region ThreadIndex

                        if (!indexMapper.ContainsKey(unit.ThreadId))
                        {
                            indexMapper.Add(unit.ThreadId, i++);
                        }
                        unit.ThreadIndex = indexMapper[unit.ThreadId];

                        #endregion // ThreadIndex

                        if (ParallelWorkUnitTrace.Floor > unit.StartAt)
                            ParallelWorkUnitTrace.Floor = unit.StartAt;
                    }

                    #endregion // Modify result

                    _threadCount = indexMapper.Count;
                    TotalMilliseconds = _watch.ElapsedMilliseconds;
                }

            });
            t.ContinueWith(tsk =>
            {
                _isProcessing = false;
                PropertyChanged(this, new PropertyChangedEventArgs("IsProcessing"));
                PropertyChanged(this, new PropertyChangedEventArgs("CanvasWidth"));
                Refresh();
            }, dispatcherScheduler);

            //Refresh();
        }

        #endregion // Start

        #region Refresh

        private void Refresh()
        {
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion // Refresh

        #region ExecuteWorkUnit

        public void ExecuteWorkUnit(DataItem unit)
        {
            ParallelWorkUnitTrace.Create(unit, SelectedDataCommand.HeightFactor, _watch, this, () => _arrangeBy);
        }

        #endregion // ExecuteWorkUnit

        #endregion // Public Methods

        public void OnImportsSatisfied()
        {
            SelectedDataCommand = DataCommands.OrderBy(item => item.Order).First();
        }
    }
}
