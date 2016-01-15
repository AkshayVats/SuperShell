using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SuperShell.Core.Workspace
{
    static class Workspace
    {
        public static readonly string Root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SuperShell";
        internal static string Name;
        static ConsoleWriter consoleWriter = new ConsoleWriter();
        static TabItem outputTab;
        public static void CreateWorkspace(string name)
        {
            WorkspaceModel model = WorkspaceModel.NewWorkspace(name);
            consoleWriter.TextChange += ConsoleWriter_TextChange;

            Plug.PluginManager.Init(Core.Evaluator.Inst);
            Actions.ActionManager.RefreshActions();

            Name = model.Name;
            consoleWriter.Write(model.ConsoleOutput);
            Parallel.ForEach(model.Assemblies, (path) =>
            {
                Evaluator.Inst.LoadAssembly(path);
            });
            Ui.Host.Tabs.Items.Clear();
            foreach (var winModel in model.Windows)
            {
                if (winModel.type == WindowModel.WindowType.Interactive)
                {
                    Ui.Host.Tabs.Items.Add(InteractiveWindowModel.GenerateTab(winModel as InteractiveWindowModel));
                }
            }
            Ui.Host.Tabs.Items.Add(new TabItem() { Content = new Ui.Main.NewTabControl(), Header = "New" });
            Ui.Host.Tabs.SelectedIndex = 0;
        }
        public static void Save()
        {
            var dir = Root + "\\" + Name;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var model = new WorkspaceModel();
            model.Name = Name;
            model.Assemblies = Evaluator.Inst.LoadedAssemblyLocations;
            model.CommandHistory = 

        }
        private static void ConsoleWriter_TextChange(object sender, EventArgs e)
        {
            if (outputTab != null)
            {
                var outputWindow = outputTab.Content as Ui.OutputWindow;
                outputWindow.txtConsole.Text = consoleWriter.Text;
            }
        }

        static internal void NewInteractiveWindow()
        {
            Ui.Host.Tabs.Items.Insert(0, new TabItem() { Content = new Ui.ShellTab(), Header = "C# Interactive" });
            Ui.Host.Tabs.SelectedIndex = 0;
        }
        internal static void AddConsoleMessage(string msg)
        {
            consoleWriter.WriteLine(msg);
        }
        internal static void OpenConsoleWindow()
        {
            if (outputTab == null)
            {
                outputTab = new TabItem();
                var outputWindow = new Ui.OutputWindow();
                outputTab.Content = outputWindow;
                outputTab.Header = "Console";
                outputWindow.txtConsole.Text = consoleWriter.Text;
            }
            Ui.Host.Tabs.Items.Insert(0, outputTab);
            Ui.Host.Tabs.SelectedIndex = 0;
        }
    }
}
