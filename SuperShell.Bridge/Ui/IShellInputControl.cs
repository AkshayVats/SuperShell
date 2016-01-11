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
        event EventHandler<string> CommandEntered;
        bool? Evaluating { get; set; }
        Control Control { get; }
        int CaretOffset { get; set; }
        string Text { get; set; }
    }
}
