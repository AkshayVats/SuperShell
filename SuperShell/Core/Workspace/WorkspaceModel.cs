using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SuperShell.Core.Workspace
{
    class WorkspaceModel
    {
        public string Name;
        public string ConsoleOutput;
        public string Usings;
        public string[] Assemblies;
        public WindowModel[] Windows;
        public string[] CommandHistory;
        public static WorkspaceModel NewWorkspace(string name)
        {
            var model = new WorkspaceModel();
            model.Name = name;
            model.ConsoleOutput = "";
            model.Usings = "using SuperShell.Util;using System;using System.Net;using System.Collections.Generic;";
            model.Assemblies = new string[]
            {
                typeof(Util.Shell).Assembly.Location,                                   //Current (Shell)
                typeof(Button).Assembly.Location,                                       //PresentationFramework
                typeof(System.Windows.Media.Imaging.BitmapSource).Assembly.Location     //PresentationCore

            };
            model.Windows = new WindowModel[]
            {
                InteractiveWindowModel.NewWindowModel(new List<string>() {"" })
            };
            model.CommandHistory = new string[0];
            return model;
        }
    }
    class WindowModel
    {
        internal enum WindowType { Interactive, Console, Source}
        public WindowType type;
    }
    class InteractiveWindowModel : WindowModel
    {
        public List<string> Inputs;
        public InteractiveWindowModel()
        {
            type = WindowType.Interactive;
        }
        internal static InteractiveWindowModel NewWindowModel(List<string> inputs)
        {
            var model = new InteractiveWindowModel();
            model.Inputs = inputs;
            return model;
        }
        internal static TabItem GenerateTab(InteractiveWindowModel model)
        {
            var shellTab = new Ui.ShellTab();
            var tab = new TabItem() { Content = shellTab, Header = "C# Interactive" };
            shellTab.LoadInputs(model.Inputs);
            return tab;
        }
    }
}
