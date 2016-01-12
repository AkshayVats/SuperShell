using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Bridge.Core
{
    public interface ICommandHistoryManager
    {
        string Previous();
        string Next();
        void Reset();
        void Add(string str);
    }
}
