using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SuperShell.Core.Workspace;

namespace SuperShell.Ui
{
    /// <summary>
    /// Interaction logic for OutputWindow.xaml
    /// </summary>
    public partial class OutputWindow : UserControl, Core.Workspace.WorkspaceTab
    {
        public OutputWindow()
        {
            InitializeComponent();
        }

        string WorkspaceTab.GetFilePath()
        {
            return null;
        }

        WindowModel.WindowType WorkspaceTab.GetWindowType()
        {
            return WindowModel.WindowType.Console;
        }

        void WorkspaceTab.Load(string filePath)
        {
           
        }

        void WorkspaceTab.Save()
        {
            
        }
    }
}
