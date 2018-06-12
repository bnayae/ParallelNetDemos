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
    public abstract class DataPluginBase : IDataPlugin
    {
        private const double DEFAULT_HEIGHT_FACTOR = 10;

        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract double Order { get; }

        [Import]
        protected IViewModel ViewModel { get; set; }

        public virtual double HeightFactor { get { return DEFAULT_HEIGHT_FACTOR; } }

        public abstract IEnumerable<DataItem> DataCollection { get; }
    }
}
