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
            return obj.Length>0;
        }

        public void Invoke(object[] obj, IObjectViewer viewer)
        {
            if (obj.Length == 0) return;
            var card = ActionManager.FindAncestor<Ui.Interactive.ICardManager>(viewer);
            string objName;
            if(obj.Length==1)
                objName = Core.Evaluator.Inst.RefrenceObject(obj.First());
            else objName = Core.Evaluator.Inst.RefrenceObject(obj);
            card?.GetLastCard().AppendInput(objName);
        }
    }
}
