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

namespace SuperShell.Ui
{
    /// <summary>
    /// Interaction logic for ShellTab.xaml
    /// </summary>
    public partial class ShellTab : UserControl, ICardManager
    {
        public ShellTab()
        {
            InitializeComponent();
            AddNewShellCard();
        }

        public void AddEmptyCard()
        {
            var lastCard = GetLastCard();
            if (lastCard.IsEvaluated&&lastCard.HasOutput)
                AddNewShellCard();
        }
        public ShellCard GetLastCard()
        {
            var lastCard = panel.Children[panel.Children.Count - 1] as ShellCard;
            return lastCard;
        }

        private void AddNewShellCard()
        {
            var card = new ShellCard(this);
            panel.Children.Add(card);
            card.Focus();
        }
    }
}
