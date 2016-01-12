using SuperShell.Core;
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
using System.Windows.Threading;

namespace SuperShell.Ui
{
    /// <summary>
    /// Interaction logic for InputControl.xaml
    /// </summary>
    public partial class ShellCard : UserControl
    {
        private ICardManager _cardManager;
        public bool HasOutput { get; private set; }
        public bool IsEvaluated { get; private set; }
        private Bridge.Ui.IShellInputControl input;
        internal ShellCard(ICardManager cardManager):base()
        {
            InitializeComponent();
            AddEditor();
            _cardManager = cardManager;
        }

        private void AddEditor()
        {
            input = Plug.PluginManager.CodeEditorProvider.GenerateShellInput();
            var editorUi = input.Control;
            editorUi.Padding = new Thickness(5);
            input.CommandEntered += ShellInputControl_CommandEntered;
            grid.Children.Add(editorUi);
        }

        private void ShellInputControl_CommandEntered(object sender, string e)
        {
            EvaluateCommand(e);
        }

        private void EvaluateCommand(string e)
        {
            input.Evaluating = true;
            IsEvaluated = false;
            input.Control.Style = FindResource("shell_evaluating") as Style;
            ClearCardOutput();
            var result = Evaluator.Inst.Evaluate(e);
            if (result.Errors?.Count() > 0)
            {
                var output = new OutputCard();
                output.RenderMessage(String.Join("\n", result.Errors), OutputCard.OutputType.Error);
                stackPanel.Children.Add(output);
                input.Evaluating = null;
                input.Control.Style = FindResource("shell_active") as Style;
            }
            else
            {
                if (result.Result != null)
                {
                    var output = new OutputCard();
                    output.Render(result.Result, OutputCard.OutputType.Normal);
                    stackPanel.Children.Add(output);
                }

                IsEvaluated = true;
                input.Control.Style = FindResource("shell_inactive") as Style;
                HasOutput = true;
                input.Evaluating = false;
                _cardManager.AddEmptyCard();
            }
        }

        private void ClearCardOutput()
        {
            HasOutput = true;
            while (stackPanel.Children.Count > 1)
                stackPanel.Children.RemoveAt(1);
        }
        public new void Focus()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input,
                new Action(delegate () {
                    input.Control.Focus();         // Set Logical Focus
                    Keyboard.Focus(input.Control); // Set Keyboard Focus
                }));
        }
        public void AppendInput(string str)
        {
            int z = input.CaretOffset + str.Length;
            input.Text = input.Text.Insert(input.CaretOffset, str);
            input.CaretOffset = z;
        }
    }
}
