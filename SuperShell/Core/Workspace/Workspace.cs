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

        /// <summary>
        /// Create workspace with given name
        /// </summary>
        /// <param name="name">Name of the workspace</param>
        public static void CreateWorkspace(string name)
        {
            InitEvaluator();
            WorkspaceModel model = WorkspaceModel.NewWorkspace(name);
            Load(model);
            
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

        }

        private static void Load(WorkspaceModel model)
        {
            Name = model.Name;
            
            //Set title of the main window
            Actions.ActionManager.FindAncestor<System.Windows.Window>(Ui.Host.Tabs).Title = "#"+Name;

            consoleWriter.Write(model.ConsoleOutput);               //restore previous console text

            //parallelly load all the assemblies
            Task.Factory.StartNew(() => {
                Parallel.ForEach(model.Assemblies, (path) =>
                {
                    Evaluator.Inst.LoadAssembly(path);
                });
                Evaluator.Inst.Evaluate(model.Usings);              //restore usings
            });
            
            //Load command history
            CommandHistoryManager.Inst.Commands = new LinkedList<string>( model.CommandHistory);

            //setup ui
            Ui.Host.Tabs.Items.Clear();
            foreach (var winModel in model.Windows)
            {
                if (winModel.WinType == WindowModel.WindowType.Interactive)
                {
                    var tab = new Ui.Interactive.ShellTab();
                    tab.Load(winModel.FilePath);
                    Ui.Host.Tabs.Items.Add(new TabItem() { Content = tab, Header = "C# Interactive" });
                }
                else if(winModel.WinType == WindowModel.WindowType.Text)
                {
                    var tab = new Ui.Main.TextTab();
                    tab.Load(winModel.FilePath);
                    Ui.Host.Tabs.Items.Add(new TabItem() { Content = tab, Header = "Text" });
                }
            }
            Ui.Host.Tabs.Items.Add(new TabItem() { Content = new Ui.Main.NewTabControl(), Header = "New" });
            Ui.Host.Tabs.SelectedIndex = 0;
            _hasWorkspace = true;

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
            model.ConsoleOutput = consoleWriter.ToString();
            model.Usings = Evaluator.Inst.GetUsing();

            var windows = new List<WindowModel>();
            for(int i = 0; i < Ui.Host.Tabs.Items.Count - 1; i++)
            {
                var tabItem = (Ui.Host.Tabs.Items[i] as TabItem);
                var filePath = dir + "\\file" + i;
                if(tabItem.Header.ToString() == "C# Interactive")
                {
                    var tab = tabItem.Content as Ui.Interactive.ShellTab;
                    windows.Add(new WindowModel() { WinType = WindowModel.WindowType.Interactive, FilePath = filePath });
                    tab.Save(filePath);
                }else if(tabItem.Header.ToString() == "Text")
                {
                    var tab = tabItem.Content as Ui.Main.TextTab;
                    windows.Add(new WindowModel() { WinType = WindowModel.WindowType.Text, FilePath = filePath });
                    tab.Save(filePath);
                }
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
            if (outputTab != null)
            {
                var outputWindow = outputTab.Content as Ui.OutputWindow;
                outputWindow.txtConsole.Text = consoleWriter.Text;
            }
        }

        static internal void NewInteractiveWindow()
        {
            Ui.Host.Tabs.Items.Insert(0, new TabItem() { Content = new Ui.Interactive.ShellTab(), Header = "C# Interactive" });
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
        internal static void NewTextWindow()
        {
            Ui.Host.Tabs.Items.Insert(Ui.Host.Tabs.Items.Count-1, new TabItem() { Content = new Ui.Main.TextTab(), Header = "Text" });
            Ui.Host.Tabs.SelectedIndex = Ui.Host.Tabs.Items.Count - 2;
        }
    }
}
