using SuperShell.Bridge.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ICSharpCode.CodeCompletion
{
    public class ShellControl:CodeTextEditor, SuperShell.Bridge.Ui.IShellInputControl
    {
        private bool _resetCommandHistory;
        private ICommandHistoryManager _lastCommands;
        public event EventHandler<string> CommandEntered;
        public event EventHandler<string> CommandAltEntered;

        private bool? _evaluating;
        public void SetCommandHistoryManager(ICommandHistoryManager manager)
        {
            _lastCommands = manager;
        }
        public ShellControl()
        {
            PreviewKeyDown += CodeTextEditor_PreviewKeyDown;
            ShowLineNumbers = false;
        }

        private void CodeTextEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (completionWindow == null)
            {
                if (!IsReadOnly && e.Key == Key.Enter || (e.Key == Key.System && e.SystemKey == Key.Enter))
                {
                    if (e.Key == Key.Enter)
                        CommandEntered?.Invoke(this, Text);
                    else
                        CommandAltEntered?.Invoke(this, Text);
                    e.Handled = true;
                    _lastCommands.Add(Text);
                    if (_resetCommandHistory)
                        _lastCommands.Reset();
                }
                if (insightWindow == null)
                {
                    if (e.Key == Key.Up)
                    {
                        var code = _lastCommands.Previous();
                        if (code != null)
                        {
                            Clear();
                            AppendText(code);
                        }
                        _resetCommandHistory = false;
                    }
                    else if (e.Key == Key.Down)
                    {
                        var code = _lastCommands.Next();
                        if (code != null)
                        {
                            Clear();
                            AppendText(code);
                        }
                        _resetCommandHistory = false;
                    }
                    else _resetCommandHistory = true;

                }
            }
        }

    }
}
