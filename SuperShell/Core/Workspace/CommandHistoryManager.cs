using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperShell.Core;

namespace SuperShell.Core.Workspace
{
    internal class CommandHistoryManager : Bridge.Core.ICommandHistoryManager
    {
        static CommandHistoryManager _inst;
        internal static CommandHistoryManager Inst
        {
            get
            {
                if(_inst==null)
                {
                    _inst = new CommandHistoryManager();
                    _inst.node = _inst.Commands.Last;
                }
                return _inst;
            }
        }
        internal LinkedList<string> Commands = new LinkedList<string>();
        LinkedListNode<string> node;
        const int MaxCommands = 100;
        private string Get()
        {
            return node?.Value;
        }
        public void Add(string str)
        {
            Commands.AddLast(str);
            if (Commands.Count > MaxCommands)
            {
                if (node == Commands.First) node = Commands.Last;
                Commands.RemoveFirst();
            }
        }

        public string Next()
        {
            if (node?.Next == null) return null;
            node = node.Next;
            return node.Value;
        }

        public string Previous()
        {
            if(node?.Previous!=null)
                node = node.Previous;
            if (node == null) node = Commands.Last;
            return node?.Value;
        }

        public void Reset()
        {
            node = null;
        }
    }
}
