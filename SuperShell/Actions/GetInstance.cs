using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperShell.Output.Viewers;
using System.Windows;

namespace SuperShell.Actions
{
    class GetInstance : IAction
    {
        public string Category
        {
            get
            {
                return null;
            }
        }

        public string Title
        {
            get
            {
                return "Get Instance";
            }
        }

        public Type SupportedType
        {
            get
            {
                return typeof(object);
            }
        }

        public void Invoke(object obj, IObjectViewer viewer)
        {
            object z = viewer;
            while(!(z is Ui.ICardManager))
            {
                z = (z as FrameworkElement).Parent;
                if (z == null) return;
            }
            (z as Ui.ICardManager).GetLastCard().AppendInput(Core.Evaluator.Inst.RefrenceObject(obj));
        }
    }
}
