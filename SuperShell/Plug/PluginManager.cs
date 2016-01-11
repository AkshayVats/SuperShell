using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Plug
{
    static class PluginManager
    {
        static readonly string location = Environment.CurrentDirectory + "/Plugs/";
        public static Bridge.Plug.ICodeEditorProvider CodeEditorProvider;
        public static void Init(Bridge.Core.IShell shell)
        {
            var typ = GetInterfaceImplementor<Bridge.Plug.ICodeEditorProvider>(location).FirstOrDefault();
            if(typ!=null)
            {
                CodeEditorProvider = Activator.CreateInstance(typ) as Bridge.Plug.ICodeEditorProvider;
                Task.Factory.StartNew(() => { CodeEditorProvider.InitCompletion(shell); });
                
            }
        }
        public static Type[] GetInterfaceImplementor<T>(string directory)
        {
            if (String.IsNullOrEmpty(directory)) { return null; } //sanity check

            DirectoryInfo info = new DirectoryInfo(directory);
            if (!info.Exists) { return null; } //make sure directory exists

            var implementors = new List<Type>();

            foreach (FileInfo file in info.GetFiles("*.dll")) //loop through all dll files in directory
            {
                Assembly currentAssembly = null;
                try
                {
                    var name = AssemblyName.GetAssemblyName(file.FullName);
                    currentAssembly = Assembly.Load(name);
                }
                catch (Exception ex)
                {
                    continue;
                }
                implementors.AddRange(
                currentAssembly.GetTypes()
                    .Where(t => t != typeof(T) && typeof(T).IsAssignableFrom(t)));
            }
            return implementors.ToArray();
        }
    }
}
