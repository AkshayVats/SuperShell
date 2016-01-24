using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SuperShell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.RequestingAssembly == null)
                return null;
            var an = new System.Reflection.AssemblyName(args.Name);
            return AppDomain.CurrentDomain.GetAssemblies()
                .Select(i => new { Assembly = i, AN = new System.Reflection.AssemblyName(i.FullName) })
                .FirstOrDefault(i => i.AN.GetPublicKeyToken().SequenceEqual(an.GetPublicKeyToken())&& i.AN.Name == an.Name)?.Assembly;

        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Core.Workspace.Workspace.Save();
        }
    }
}
