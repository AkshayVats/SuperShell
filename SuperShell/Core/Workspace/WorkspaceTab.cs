using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Core.Workspace
{
    internal interface WorkspaceTab
    {
        void Load(string filePath);
        void Save();
        WindowModel.WindowType GetWindowType();
        string GetFilePath();
    }
}
