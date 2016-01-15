using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Core.Workspace
{
    class ConsoleWriter:TextWriter
    {
        public event EventHandler TextChange;
        public ConsoleWriter()
        {
            Console.SetOut(this);
        }
        public string Text { get; private set; }

        public override Encoding Encoding
        {
            get
            {
                return null;
            }
        }

        public override void Write(char value)
        {
            base.Write(value);
            Text += value;
            TextChange?.Invoke(this, null);
        }
        public override void Write(string value)
        {
            //base.Write(value);
            Text += value;
            TextChange?.Invoke(this, null);
        }
        public override void WriteLine(string value)
        {
            //base.WriteLine(value);
            Text += value+"\n";
            TextChange?.Invoke(this, null);
        }
    }
}
