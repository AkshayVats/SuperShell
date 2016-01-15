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

namespace SuperShell.Ui.Main
{
    /// <summary>
    /// Interaction logic for WorkspaceTab.xaml
    /// </summary>
    public partial class WorkspaceTab : UserControl
    {
        public WorkspaceTab()
        {
            InitializeComponent();
        }

        private void btnNewWorkspace_Click(object sender, RoutedEventArgs e)
        {
            Core.Workspace.Workspace.LoadWorkspace(Core.Workspace.WorkspaceModel.NewWorkspace(""));
        }
    }
}
