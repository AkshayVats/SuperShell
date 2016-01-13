using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperShell.Output.Viewers;
using System.Windows.Controls;

namespace SuperShell.Actions
{
    class ParseAsImage : IAction
    {
        public string Category
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Title
        {
            get
            {
                return "Parse As Image";
            }
        }

        public bool CanInvoke(object[] obj)
        {
            return obj.All(k => Check(k));
        }

        private bool Check(object k)
        {
            return typeof(Uri).IsAssignableFrom(k.GetType());
        }

        public void Invoke(object[] obj, IObjectViewer viewer)
        {
            var manager = ActionManager.FindAncestor<Ui.ICardManager>(viewer);
            foreach (var path in obj.Cast<Uri>())
            {
                try
                {
                    manager.GetLastCard().AddOutputCard(new Image() { Source= Util.Shell.CreateBitmap(path) });
                }
                catch 
                {

                }
            }
        }
    }
}
