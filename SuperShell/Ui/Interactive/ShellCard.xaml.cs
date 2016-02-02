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

namespace SuperShell.Ui.Interactive
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
            input.SetCommandHistoryManager(Core.Workspace.CommandHistoryManager.Inst);
            var editorUi = input.Control;
            editorUi.Padding = new Thickness(5);
            input.CommandEntered += ShellInputControl_CommandEntered;
            input.CommandAltEntered += Input_CommandAltEntered;
            input.TextChanged += Input_TextChanged;
            grid.Children.Add(editorUi);

            editorUi.PreviewKeyUp += EditorUi_PreviewKeyUp;
        }

        private void Input_CommandAltEntered(object sender, string e)
        {
            EvaluateCommandAsync(e);
        }

        

        private void Input_TextChanged(object sender, EventArgs e)
        {
            if (IsEvaluated)
            {
                input.Control.Style = FindResource("shell_active") as Style;
                IsEvaluated = false;
            }
        }

        private void EditorUi_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control) //Ctrl+V...file drop
            {
                if (Clipboard.ContainsFileDropList())
                    AddOutputCard(Clipboard.GetFileDropList().Cast<string>().Select(i=>new Uri(i)).ToList());
                else if (Clipboard.ContainsImage())
                    AddOutputCard(Clipboard.GetImage());
            }
        }

        private void ShellInputControl_CommandEntered(object sender, string e)
        {
            EvaluateCommand(e);
        }
        private async void EvaluateCommandAsync(string e)
        {
            PreEvaluate();
            Evaluator.CompilerResult result = null;
            await Task.Factory.StartNew(() => {
                result = Evaluator.Inst.Evaluate(e);
            });
            
            PostEvaluate(result);
        }
        private void EvaluateCommand(string e)
        {
            PreEvaluate();
            var result = Evaluator.Inst.Evaluate(e);
            PostEvaluate(result);
        }

        private void PostEvaluate(Evaluator.CompilerResult result)
        {
            if (result.Errors?.Count() > 0)
            {
                var output = new OutputCard();
                output.RenderMessage(String.Join("\n", result.Errors), OutputCard.OutputType.Error);
                stackPanel.Children.Add(output);
                input.SetReadOnly(false);
                input.Control.Style = FindResource("shell_active") as Style;
            }
            else
            {
                if (result.Result != null)
                {
                    AddOutputCard(result.Result);
                }

                IsEvaluated = true;
                input.SetReadOnly(false);
                _cardManager.AddEmptyCard();
            }
        }

        private void PreEvaluate()
        {
            input.SetReadOnly(true);
            IsEvaluated = false;
            input.Control.Style = FindResource("shell_evaluating") as Style;
        }

        public void AddOutputCard(object result)
        {
            var output = new OutputCard();
            output.Render(result, OutputCard.OutputType.Normal);
            output.Crossed += (o, e) => { stackPanel.Children.Remove(o as UIElement); };
            stackPanel.Children.Add(output);
            input.Control.Style = FindResource("shell_inactive") as Style;
            HasOutput = true;
        }

        private void ClearCardOutput()
        {
            HasOutput = false;
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

        internal string GetInput()
        {
            return input.Text;
        }
        internal void SetInput(string code)
        {
            input.Text = code;
        }
    }
}
