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
using System.IO;

namespace SuperShell.Ui.Main
{
    /// <summary>
    /// Interaction logic for SourceTab.xaml
    /// </summary>
    public partial class SourceTab : UserControl, Core.Workspace.WorkspaceTab
    {
        string _filePath;
        SuperShell.Bridge.Ui.ICodeEditorControl code;
        public SourceTab()
        {
            InitializeComponent();
            
        }

        string Core.Workspace.WorkspaceTab.GetFilePath()
        {
            return _filePath;
        }

        WindowModel.WindowType Core.Workspace.WorkspaceTab.GetWindowType()
        {
            return WindowModel.WindowType.Source;
        }

        void Core.Workspace.WorkspaceTab.Load(string filePath)
        {
            _filePath = filePath;
            code = Plug.PluginManager.CodeEditorProvider.GenerateCodeEditor(filePath);
            grid.Children.Add(code.Control);
            if (File.Exists(filePath))
            {
                code.Text = File.ReadAllText(filePath);
                Compile();
            }
        }

        void Core.Workspace.WorkspaceTab.Save()
        {
            if(code!=null)
                File.WriteAllText(_filePath, code.Text);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Compile();
        }

        private void Compile()
        {
            if (code != null)
                Core.Evaluator.Inst.CompileDocument(code.Text, _filePath);
        }
    }
}
