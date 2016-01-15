using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SuperShell.Bridge.Ui
{
    public interface IShellInputControl
    {
        event EventHandler TextChanged;
        event EventHandler<string> CommandEntered;
        void SetReadOnly(bool readOnly);
        Control Control { get; }
        int CaretOffset { get; set; }
        string Text { get; set; }
        void SetCommandHistoryManager(Core.ICommandHistoryManager manager);
    }
}
