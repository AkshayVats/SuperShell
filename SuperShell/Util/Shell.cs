using SuperShell.Ui.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SuperShell.Util
{
    public static class Shell
    {
        public static List<Core.Assemblies.AssemblyViewModel> ListGacAssemblies()
        {
            return Core.Assemblies.GAC.GetAssemblyList().ToList()
                .AsParallel()
                .Select(i => new Core.Assemblies.AssemblyViewModel(i))
                .Where(i=>i.Path!=null)
                .ToList();
        }
        public static BitmapImage CreateBitmap(Uri path)
        {
            return new BitmapImage(path);
        }
        public static List<Core.Assemblies.AssemblyViewModel> GetAssemblies()
        {
            return Core.Assemblies.FrameworkAssemblies.ListAssemblies("v4.5.2").ToList();
        }
        public static void ReferenceAssemblies(params Core.Assemblies.AssemblyViewModel[] assemblies)
        {
            foreach (var a in assemblies)
                Core.Evaluator.Inst.LoadAssembly(a.Path);
            Console.WriteLine("Assemblies referenced");
        }
        public static List<Core.Assemblies.AssemblyViewModel> GetReferencedAssemblies()
        {
            return Core.Evaluator.Inst.LoadedAssemblyLocations
                .Select(i => new Core.Assemblies.AssemblyViewModel(i))
                .ToList();
        }
        public static void Clear()
        {
            ((Ui.Host.Tabs.SelectedItem as TabItem)?.Content as ShellTab)?.ClearAll();
        }
    }
}
