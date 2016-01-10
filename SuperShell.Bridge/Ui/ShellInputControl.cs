using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SuperShell.Bridge.Ui
{
    public interface ShellInputControl
    {
        event EventHandler<string> CommandEntered;
        bool? Evaluating { get; set; }
    }
}
