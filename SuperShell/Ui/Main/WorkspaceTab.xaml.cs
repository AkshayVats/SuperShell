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
            // var a=YoutubeExtractor.DownloadUrlResolver.GetDownloadUrls("https://www.youtube.com/watch?v=i2GC06euEDE");
            //Properties.Settings.Default.Reset();
            //var rec = Properties.Settings.Default.Recent;
            //var l = Properties.Settings.Default.LastWorkspace;
            Loaded += WorkspaceTab_Loaded;
            var last = Properties.Settings.Default.LastWorkspace;
            if (last != null && last != "") return;
                InitializeComponent();
            listbox.ItemsSource = Properties.Settings.Default.Recent;
            if (listbox.HasItems) lEmpty.Visibility = Visibility.Collapsed;
            
        }

        private async void WorkspaceTab_Loaded(object sender, RoutedEventArgs e)
        {
            var last = Properties.Settings.Default.LastWorkspace;
            if (last != null && last != "")
            {
                if (await Core.Workspace.Workspace.Load(last))
                    return;
            }
        }

        private void btnNewWorkspace_Click(object sender, RoutedEventArgs e)
        {
            var name = txtWorkspaceName.Text;
            if (name == "") name = Core.Workspace.Workspace.DEFAULT_WORKSPACE;
            Core.Workspace.Workspace.CreateWorkspace(name);
        }

        private async void listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Core.Workspace.Workspace.Load(listbox.SelectedItem as string);
        }
    }
}
