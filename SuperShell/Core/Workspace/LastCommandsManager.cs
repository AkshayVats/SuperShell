using SuperShell.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Core.Workspace
{
    static class LastCommandsManager
    {
        internal static CommandHistoryManager Inst = new CommandHistoryManager();
        static int index = -1;
        const int MaxCommands = 100;
        public static string Previous()
        {
            index--;
            return Get();
        }
        public static string Next()
        {
            index++;
            return Get();
        }
        public static void Reset()
        {
            index = -1;
        }
        public static void Add(string str)
        {
            Settings.Default.Commands.Add(str);
            while (Settings.Default.Commands.Count > MaxCommands)
            {
                Settings.Default.Commands.RemoveAt(0);
                index--;
            }
            if (index < -1) index = -1;
            Settings.Default.Save();
            //Reset();
        }
        private static string Get()
        {
            index = (index + Settings.Default.Commands.Count + 1) % (Settings.Default.Commands.Count + 1);
            if (index == Settings.Default.Commands.Count)
                return "";
            return Settings.Default.Commands[index];
        }
    }
}
