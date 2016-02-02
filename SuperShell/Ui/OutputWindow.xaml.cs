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

        internal void AppendText(string e)
        {
            if (e.Trim() == "") return;
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => {
                    AppendText(e);
                    return;
                });
            }
            var text = string.Join(Environment.NewLine, txtConsole.Inlines.Cast<Run>().Select(i => i.Text.Trim()));
            txtConsole.Inlines.Clear();
            txtConsole.Inlines.Add(new Run(text+Environment.NewLine) { Foreground = new SolidColorBrush(Colors.WhiteSmoke) });
            txtConsole.Inlines.Add(new Run(e.Trim()) { Foreground = new SolidColorBrush(Colors.Yellow) });
            scroller.ScrollToEnd();
        }
    }
}
