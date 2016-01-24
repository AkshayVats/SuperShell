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
        internal const string DEFAULT_WORKSPACE = "SuperShell";     //used when no workspace name is provided
        
        //Root workspace folder where all files are saved, currently can't be changed
        public static readonly string Root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SuperShell";

        internal static string Name;                                //Workspace Name
        static ConsoleWriter consoleWriter;                         //For console window
        static TabItem outputTab;                                   //Console window tab, there is only one such tab, we save its reference
        static bool _hasWorkspace;                                  //have we loaded any workspace, Name = null
        static List<string> workspaceFiles;                         //Path of workspace files

        /// <summary>
        /// Create workspace with given name
        /// </summary>
        /// <param name="name">Name of the workspace</param>
        public static async void CreateWorkspace(string name)
        {
            var proj = Root + "\\" + name+"\\proj.ssw";
            if (File.Exists(proj))
                await Load(proj);
            else
            {
                InitEvaluator();
                WorkspaceModel model = WorkspaceModel.NewWorkspace(name);
                Load(model);
            }
        }

        /// <summary>
        /// Everytime evaluator reference needs to get reset
        /// </summary>
        private static void InitEvaluator()
        {
            consoleWriter = new ConsoleWriter();                    //Create new console writer
            consoleWriter.TextChange += ConsoleWriter_TextChange;
            Evaluator.NewInstance();
            Plug.PluginManager.Init(Evaluator.Inst);                //plugins use reference to evaluator
            Actions.ActionManager.RefreshActions();                 //redundant (one time init will suffice)
            workspaceFiles = new List<string>();
        }

        private static void Load(WorkspaceModel model)
        {
            Name = model.Name;
            _hasWorkspace = true;
            //Set title of the main window
            Actions.ActionManager.FindAncestor<System.Windows.Window>(Ui.Host.Tabs).Title = "#"+Name;

            consoleWriter.Write(model.ConsoleOutput);               //restore previous console text

            //parallelly load all the assemblies
            Task.Factory.StartNew(() => {
                Parallel.ForEach(model.Assemblies, (path) =>
                {
                    try {
                        Evaluator.Inst.LoadAssembly(path);
                    }
                    catch(Exception e)
                    {

                    }
                });
                Evaluator.Inst.Evaluate(model.Usings);              //restore usings
            });
            
            //Load command history
            CommandHistoryManager.Inst.Commands = new LinkedList<string>( model.CommandHistory);

            //setup ui
            Ui.Host.Tabs.Items.Clear();
            outputTab = null;
            foreach (var winModel in model.Windows)
            {
                var tabHeader = GetTab(winModel.WinType);
                if (tabHeader == null) continue;
                var tab = tabHeader.Item1;
                if (winModel.FilePath == null)
                    winModel.FilePath = GetNewFilePath(winModel.WinType);
                tab.Load(winModel.FilePath);
                workspaceFiles.Add(winModel.FilePath);              //TODO:assert unique
                Ui.Host.Tabs.Items.Add(new TabItem() { Content = tab, Header = tabHeader.Item2 });
            }
            Ui.Host.Tabs.Items.Add(new TabItem() { Content = new Ui.Main.NewTabControl(), Header = "New" });
            Ui.Host.Tabs.SelectedIndex = 0;
            

        }
        private static Tuple<WorkspaceTab, string> GetTab(WindowModel.WindowType winType)
        {
            switch (winType)
            {
                case WindowModel.WindowType.Interactive:
                    return new Tuple<WorkspaceTab, string>(new Ui.Interactive.ShellTab() as WorkspaceTab, "C# Interactive");
                case WindowModel.WindowType.Text:
                    return new Tuple<WorkspaceTab, string>(new Ui.Main.TextTab() as WorkspaceTab, "Text");
                case WindowModel.WindowType.Source:
                    return new Tuple<WorkspaceTab, string>(new Ui.Main.SourceTab() as WorkspaceTab, "Source");
            }
            return null;
        }
        public static async Task<bool> Load(string path)
        {
            if (!File.Exists(path))
                return false;

            InitEvaluator();                                        //recreate the evaluator

            using (var sr = new StreamReader(path))
            {
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkspaceModel>(await sr.ReadToEndAsync());
                Load(model);
            }

            
            return true;
        }
        public static void Save()
        {
            if (!_hasWorkspace) return;
            
            var dir = Root + "\\" + Name;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            //setup workspace model
            var model = new WorkspaceModel();
            model.Name = Name;
            model.Assemblies = Evaluator.Inst.LoadedAssemblyLocations;
            model.CommandHistory = CommandHistoryManager.Inst.Commands.ToArray();
            model.ConsoleOutput = consoleWriter.Text;
            model.Usings = Evaluator.Inst.GetUsing();

            var windows = new List<WindowModel>();
            for(int i = 0; i < Ui.Host.Tabs.Items.Count - 1; i++)
            {
                var tab = (Ui.Host.Tabs.Items[i] as TabItem).Content as WorkspaceTab;
                if (tab.GetWindowType() == WindowModel.WindowType.Console) continue;
                windows.Add(new WindowModel() { WinType = tab.GetWindowType(), FilePath = tab.GetFilePath() });
                tab.Save();
            }
            model.Windows = windows.ToArray();
            var project = dir + "\\proj.ssw";
            using (var sw = new System.IO.StreamWriter(project))
            {
                sw.Write(Newtonsoft.Json.JsonConvert.SerializeObject(model));
            }

            if (Name != DEFAULT_WORKSPACE&&Properties.Settings.Default.Recent.Cast<string>().Any(i=>i== project)==false)
            {
                Properties.Settings.Default.Recent.Add(project);
                Properties.Settings.Default.LastWorkspace = null;
                Properties.Settings.Default.Save();
            }
            else if(Name == DEFAULT_WORKSPACE)
            {
                Properties.Settings.Default.LastWorkspace = project;
                Properties.Settings.Default.Save();
            }
        }
        /// <summary>
        /// Get current workspace path
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentPath()
        {
            if (!_hasWorkspace) return null;
            return Root + "\\" + Name;
        }
        internal static void Close()
        {
            if(Name == DEFAULT_WORKSPACE)                           
            {
                Directory.Delete(GetCurrentPath(), true);           //Delete folder to remove packages
            }
            else Save();

            Properties.Settings.Default.LastWorkspace = null;
            Properties.Settings.Default.Save();
            outputTab = null;
            _hasWorkspace = false;
            Name = null;
            consoleWriter.Flush();

            Ui.Host.Tabs.Items.Clear();
            Ui.Host.Tabs.Items.Add(new TabItem() { Content = new Ui.Main.WorkspaceTab(), Header = "Home" });
            Ui.Host.Tabs.SelectedIndex = 0;
        }
        private static void ConsoleWriter_TextChange(object sender, EventArgs e)
        {
            if (outputTab == null)
                OpenConsoleWindow(false);
            var outputWindow = outputTab.Content as Ui.OutputWindow;
            outputWindow.txtConsole.Text = consoleWriter.Text;
        }

        static internal void NewInteractiveWindow()
        {
            var tab = new Ui.Interactive.ShellTab() as WorkspaceTab;
            tab.Load(GetNewFilePath(WindowModel.WindowType.Interactive));
            Ui.Host.Tabs.Items.Insert(0, new TabItem() { Content = tab, Header = "C# Interactive" });
            Ui.Host.Tabs.SelectedIndex = 0;
        }
        internal static void AddConsoleMessage(string msg)
        {
            consoleWriter.WriteLine(msg);
        }
        internal static void OpenConsoleWindow(bool select=true)
        {
            if (outputTab == null)
            {
                outputTab = new TabItem();
                var outputWindow = new Ui.OutputWindow();
                outputTab.Content = outputWindow;
                outputTab.Header = "Console";
                outputWindow.txtConsole.Text = consoleWriter.Text;
            }
            Ui.Host.Tabs.Items.Insert(Ui.Host.Tabs.Items.Count - 1, outputTab);
            if(select)
                Ui.Host.Tabs.SelectedIndex = Ui.Host.Tabs.Items.Count - 2;
        }
        internal static void NewTextWindow()
        {
            var tab = new Ui.Main.TextTab() as WorkspaceTab;
            tab.Load(GetNewFilePath(WindowModel.WindowType.Text));
            Ui.Host.Tabs.Items.Insert(Ui.Host.Tabs.Items.Count-1, new TabItem() { Content = tab, Header = "Text" });
            Ui.Host.Tabs.SelectedIndex = Ui.Host.Tabs.Items.Count - 2;
        }
        internal static void NewSourceWindow()
        {
            var tab = new Ui.Main.SourceTab() as WorkspaceTab;
            tab.Load(GetNewFilePath(WindowModel.WindowType.Source));
            Ui.Host.Tabs.Items.Insert(Ui.Host.Tabs.Items.Count - 1, new TabItem() { Content = tab, Header = "Source" });
            Ui.Host.Tabs.SelectedIndex = Ui.Host.Tabs.Items.Count - 2;
        }
        private static string GetNewFilePath(WindowModel.WindowType winTyp)
        {
            string ext;
            if (winTyp == WindowModel.WindowType.Interactive) ext = ".csx";
            else if (winTyp == WindowModel.WindowType.Source) ext = ".cs";
            else if (winTyp == WindowModel.WindowType.Text) ext = ".txt";
            else return null;
            if (!_hasWorkspace) throw new Exception("No workspace");
            var prefix = Root + "\\" + Name+"\\file";
            for(int i=1; ; i++)
            {
                if (!workspaceFiles.Contains(prefix + i+ext))
                    return prefix + i+ext;
            }
        }
    }
}
