using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SuperShell.Ui.Avalon
{
    class ShellInputControl:ICSharpCode.AvalonEdit.TextEditor, Bridge.Ui.ShellInputControl
    {
        bool? _evaluating;
        public bool? Evaluating
        {
            get
            {
                return _evaluating;
            }

            set
            {
                IsReadOnly = false;
                _evaluating = value;
                if (_evaluating == null)
                    Style = FindResource("shell_active") as Style;
                else if (_evaluating == true)
                {
                    IsReadOnly = true;
                    Style = FindResource("shell_evaluating") as Style;
                }
                else Style = FindResource("shell_inactive") as Style;
            }
        }

        public ShellInputControl()
        {
            FontFamily = new System.Windows.Media.FontFamily("Consolas");
            SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("C#");
            HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
            VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Disabled;
            PreviewKeyDown += ShellInputControl_PreviewKeyDown;
        }

        private void ShellInputControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
            {
                CommandEntered?.Invoke(this, Text);
                e.Handled = true;
            }
        }

        public event EventHandler<string> CommandEntered;
    }
}
