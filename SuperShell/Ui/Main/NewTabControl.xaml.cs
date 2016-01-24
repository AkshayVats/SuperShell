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
    /// Interaction logic for NewTabControl.xaml
    /// </summary>
    public partial class NewTabControl : UserControl
    {
        public NewTabControl()
        {
            InitializeComponent();
            AddItems();
        }

        private void AddItems()
        {
            var items = new List<ImageTextViewModel>()
            {
                new ImageTextViewModel() {Text="Interactive Window", Image=FindResource("interactive_icon")as BitmapImage },
                new ImageTextViewModel() {Text="Output Window", Image=FindResource("output_icon")as BitmapImage },
                new ImageTextViewModel() {Text="Source File", Image=FindResource("source_icon")as BitmapImage },
                new ImageTextViewModel() {Text="Text File", Image=FindResource("source_icon")as BitmapImage },
            };
            listbox.ItemsSource = items;
        }

        private void listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (listbox.SelectedIndex)
            {
                case 0:
                    Core.Workspace.Workspace.NewInteractiveWindow();
                    break;
                case 1:
                    Core.Workspace.Workspace.OpenConsoleWindow();
                    break;
                case 2:Core.Workspace.Workspace.NewSourceWindow();
                    break;
                case 3:
                    Core.Workspace.Workspace.NewTextWindow();
                    break;
            }
            listbox.SelectedIndex = -1;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Core.Workspace.Workspace.Close();
        }
    }
}
