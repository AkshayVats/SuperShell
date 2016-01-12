using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Core.Assemblies
{
    public class AssemblyViewModel
    {
        private AssemblyName _assemblyName;
        public string AssemblyName { get; }
        public System.Version Version { get; }
        internal string Path { get; }
        public AssemblyViewModel(AssemblyName assemblyName)
        {
            _assemblyName = assemblyName;
            AssemblyName = _assemblyName.Name;
            Path = GAC.FindAssemblyInNetGac(_assemblyName);
            Version = _assemblyName.Version;
        }
    }
}
