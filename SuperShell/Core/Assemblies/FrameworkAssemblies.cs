using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Core.Assemblies
{
    class FrameworkAssemblies
    {
        internal static string GetPath(string version)
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Reference Assemblies\Microsoft\Framework\.NETFramework\"+version;
        }
        internal static IEnumerable<AssemblyViewModel> ListAssemblies(string version)
        {
            foreach(var file in System.IO.Directory.GetFiles(GetPath(version), "*.dll"))
            {
                AssemblyViewModel model = null;
                try
                {
                    model = new AssemblyViewModel(AssemblyName.GetAssemblyName(file));
                }
                catch { }
                if (model != null) yield return model;
            }
        }
    }
}
