using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Actions
{
    interface IAction
    {
        string Title { get; }
        string Category { get; }
        bool CanInvoke(object[] obj);
        void Invoke(object[] obj, Output.Viewers.IObjectViewer viewer);
    }
}
