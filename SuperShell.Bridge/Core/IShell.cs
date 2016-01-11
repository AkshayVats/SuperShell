using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Bridge.Core
{
    public interface IShell
    {
        string GetVars();
        string GetUsing();
        event EventHandler<Assembly> AssemblyReferenced;
        string[] LoadedAssemblyLocations { get; }
    }
}
