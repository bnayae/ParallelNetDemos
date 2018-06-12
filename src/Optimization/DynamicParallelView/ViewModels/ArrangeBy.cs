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

#endregion // Using

namespace Tpl.Samples
{
    public enum ArrangeBy
    {
        Core,
        Thread
    }
}
