using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SuperShell.Bridge.Ui
{
    public interface ICodeEditorControl
    {
        event EventHandler TextChanged;
        void SetReadOnly(bool readOnly);
        Control Control { get; }
        string Text { get; set; }
    }
}
