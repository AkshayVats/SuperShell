using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Core.Workspace
{
    internal class CommandHistoryManager : Bridge.Core.ICommandHistoryManager
    {
        internal LinkedList<string> Commands = new LinkedList<string>();
        int index = -1;
        const int MaxCommands = 100;
        private string Get()
        {
            
            index = (index + Settings.Default.Commands.Count + 1) % (Settings.Default.Commands.Count + 1);
            if (index == Settings.Default.Commands.Count)
                return "";
            return Settings.Default.Commands[index];
        }
        public void Add(string str)
        {
            
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
