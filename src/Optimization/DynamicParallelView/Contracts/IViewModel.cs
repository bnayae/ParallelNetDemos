#region Using

using System;
using System.Collections.Generic;

#endregion // Using

namespace Tpl.Samples
{
    public interface IViewModel
    {
        /// <summary>
        /// Gets / Sets enumerable factory that contain Tuple of index and unit execution duration
        /// </summary>
        //Func<IEnumerable<DataItem>> DataFactory { get; set; }
        void ExecuteWorkUnit(DataItem unit);
        void Start(Action action);
        int ItemsPerCore { get; }
        string CurrentDataStrategy { get; set; }
        IDataPlugin SelectedDataCommand { get; }
        event EventHandler DataSelectionChanged;
    }
}
