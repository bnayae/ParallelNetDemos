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
    public class DataItem 
    {
        public DataItem(object item, int spinDuration)
        {
            Item = item;
            SpinDuration = spinDuration;
        }
        public object Item { get; private set; }
        public long SpinDuration { get; private set; }
    }
}
