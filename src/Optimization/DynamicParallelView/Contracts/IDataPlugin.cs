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
    [InheritedExport(typeof(IDataPlugin))]
    public interface IDataPlugin
    {
        string Name { get; }
        string Description { get; }
        //ICommand Command { get; }
        double Order { get; }
        //event EventHandler DataChanged;
        double HeightFactor { get; }
        IEnumerable<DataItem> DataCollection { get; }
    }
}
