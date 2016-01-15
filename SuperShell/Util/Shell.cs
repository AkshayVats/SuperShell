using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public static void LL()
        {
            int t = 0;
        }
    }
}
