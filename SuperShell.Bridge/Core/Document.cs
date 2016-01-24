using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Bridge.Core
{
    public class Document
    {
        public string Path { get;}
        public string Text { get; }
        public Document(string path, string text)
        {
            Path = path;
            Text = text;
        }
    }
}
