using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperShell.Bridge.Core;
using SuperShell.Bridge.Ui;

namespace ICSharpCode.CodeCompletion
{
    public class AvalonPlugin : SuperShell.Bridge.Plug.ICodeEditorProvider
    {
        CSharpCompletion _completion;
        List<WeakReference<CodeTextEditor>> _editors = new List<WeakReference<CodeTextEditor>>();
        public IShellInputControl GenerateShellInput()
        {
            var avalon = new CodeTextEditor();
            if (_completion == null) _editors.Add(new WeakReference<CodeTextEditor>(avalon));
            else avalon.Completion = _completion;
            avalon.FileName = "Repl.csx";
            return avalon;
        }
        

        public void InitCompletion(IShell shell)
        {
            _completion = new CSharpCompletion(shell);
            Parallel.ForEach(_editors, (e) => {
                CodeTextEditor target;
                if(e.TryGetTarget(out target))
                {
                    target.Completion = _completion;
                }
            });
        }
    }
}
