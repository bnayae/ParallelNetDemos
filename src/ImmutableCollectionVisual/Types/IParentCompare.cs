#region Using

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#endregion // Using

namespace Sela.Samples
{
    public interface IParentCompare
    {
        bool IsExists(object item);
    }
}
