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
        public bool IsReferenced { get; }
        public AssemblyViewModel(AssemblyName assemblyName)
        {
            _assemblyName = assemblyName;
            AssemblyName = _assemblyName.Name;
            Path = GAC.FindAssemblyInNetGac(_assemblyName);
            Version = _assemblyName.Version;
            IsReferenced = Evaluator.Inst.LoadedAssemblyLocations.Any(i=>i.Equals(Path, StringComparison.OrdinalIgnoreCase));
        }
        public AssemblyViewModel(string dllPath)
        {
            _assemblyName = System.Reflection.AssemblyName.GetAssemblyName(dllPath);
            AssemblyName = _assemblyName.Name;
            Path = dllPath;
            Version = _assemblyName.Version;
            IsReferenced = Evaluator.Inst.LoadedAssemblyLocations.Any(i => i.Equals(Path, StringComparison.OrdinalIgnoreCase));
        }
    }
}
