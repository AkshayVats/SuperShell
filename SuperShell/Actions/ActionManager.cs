using SuperShell.Output.Viewers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SuperShell.Actions
{
    static class ActionManager
    {
        public static List<IAction> Actions;
        public static void RefreshActions()
        {
            Actions = new List<IAction>();
            Actions.Add(new GetInstance());
        }
        

        #region Menu
        
        public static void PopulateMenu(ContextMenu menu, IObjectViewer viewer, object[] objects)
        {
            menu.Items.Clear();
            foreach (var action in Actions.Where(i => i.CanInvoke(objects)))
            {
                var mi = new MenuItem() { Header = action.Title };
                mi.Click += (o1, e1) => { action.Invoke(objects, viewer); };
                menu.Items.Add(mi);
            }
            
        }
        #endregion

        public static T FindAncestor<T>(IObjectViewer viewer)
        {
            object z = viewer;
            while (!(z is Ui.ICardManager))
            {
                z = (z as FrameworkElement).Parent;
                if (z == null) return default(T);
            }
            return (T)z;
        }
    }
}
