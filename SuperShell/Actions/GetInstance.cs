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

        public bool CanInvoke(object[] obj)
        {
            return obj.Length==1;
        }

        public void Invoke(object[] obj, IObjectViewer viewer)
        {
            var card = ActionManager.FindAncestor<Ui.ICardManager>(viewer);
            card?.GetLastCard().AppendInput(Core.Evaluator.Inst.RefrenceObject(obj.First()));
        }
    }
}
