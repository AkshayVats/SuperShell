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
using System.Windows.Shapes;

namespace SuperShell.Ui
{
    /// <summary>
    /// Interaction logic for Host.xaml
    /// </summary>
    public partial class Host : Window
    {
        public Host()
        {
            Plug.PluginManager.Init(Core.Evaluator.Inst);
            Actions.AllActions.RefreshActions();
            InitializeComponent();
            
        }
    }
}
