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

namespace SuperShell.Ui.Interactive
{
    /// <summary>
    /// Interaction logic for ShellTab.xaml
    /// </summary>
    public partial class ShellTab : UserControl, ICardManager, Core.Workspace.WorkspaceTab
    {
        private string _filePath;
        public ShellTab()
        {
            InitializeComponent();
        }

        public void AddEmptyCard()
        {
            var lastCard = GetLastCard();
            if (lastCard==null|| lastCard.GetInput()!="")
                AddNewShellCard();
        }
        public ShellCard GetLastCard()
        {
            var lastCard = panel.Children[panel.Children.Count - 1] as ShellCard;
            return lastCard;
        }

        private ShellCard AddNewShellCard()
        {
            var card = new ShellCard(this);
            panel.Children.Add(card);
            card.Focus();
            scroller.ScrollToBottom();
            return card;
        }


       
        
        async void WorkspaceTab.Load(string filePath)
        {
            _filePath = filePath;
            if (!System.IO.File.Exists(filePath))
            {
                AddNewShellCard();
                return;
            }
            using (var sr = new System.IO.StreamReader(filePath))
            {
                var list = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(await sr.ReadToEndAsync());
                foreach (var code in list)
                {
                    AddNewShellCard().SetInput(code);
                }
            }
        }

        public void Save()
        {
            var list = new List<string>();
            foreach (var card in panel.Children.Cast<ShellCard>())
                list.Add(card.GetInput());
            using (var sw = new System.IO.StreamWriter(_filePath))
            {
                sw.Write(Newtonsoft.Json.JsonConvert.SerializeObject(list));
            }
        }

        WindowModel.WindowType WorkspaceTab.GetWindowType()
        {
            return WindowModel.WindowType.Interactive;
        }

        public string GetFilePath()
        {
            return _filePath;
        }
        public void ClearAll()
        {
            panel.Children.Clear();
            AddNewShellCard();
        }
    }
}
