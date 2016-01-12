using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SuperShell.Actions
{
    static class AllActions
    {
        public static List<IAction> Actions;
        public static void RefreshActions()
        {
            Actions = new List<IAction>();
            Actions.Add(new GetInstance());
        }
        public static IEnumerable<IAction> GetActions(Type type)
        {
            return Actions?.Where(i => i.SupportedType.IsAssignableFrom(type) );
        }

        public static void AttachMenu(object obj, Output.Viewers.IObjectViewer viewer)
        {
            var menu = GenerateMenu(obj, viewer);
            viewer.GetUi().ContextMenu = menu;
        }
        public static ContextMenu GenerateMenu(object obj, Output.Viewers.IObjectViewer viewer)
        {
            return GenerateMenu(obj.GetType(), (action) => { action.Invoke(obj, viewer); });
        }
        public static ContextMenu GenerateMenu(Type filterType, Action<IAction> onAction)
        {
            var menu = new ContextMenu();
            foreach (var action in GetActions(filterType))
            {
                var mi = new MenuItem() { Header = action.Title };
                mi.Click += (o, e) => { onAction.Invoke(action); };
                menu.Items.Add(mi);
            }
            return menu;
        }
    }
}
