using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Core
{
    internal class CommandHistoryManager : Bridge.Core.ICommandHistoryManager
    {
        public void Add(string str)
        {
            LastCommandsManager.Add(str);
        }

        public string Next()
        {
            return LastCommandsManager.Next();
        }

        public string Previous()
        {
            return LastCommandsManager.Previous();
        }

        public void Reset()
        {
            LastCommandsManager.Reset();
        }
    }
}
