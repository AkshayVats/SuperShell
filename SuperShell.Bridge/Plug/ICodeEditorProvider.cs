using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Bridge.Plug
{
    public interface ICodeEditorProvider
    {
        void InitCompletion(Core.IShell shell);
        Ui.IShellInputControl GenerateShellInput();
        Ui.ICodeEditorControl GenerateCodeEditor(string filename);
    }
}
