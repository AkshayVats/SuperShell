using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Avalon
{
    public class ShellInputControl: ICSharpCode.AvalonEdit.TextEditor, Bridge.Ui.ShellInputControl
    {
        public ShellInputControl()
        {
            SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("C#");
        }
    }
}
