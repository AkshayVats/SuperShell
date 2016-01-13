using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SuperShell.Output.Viewers
{
    class FrameworkElementViewer : IObjectViewer<FrameworkElement>
    {
        FrameworkElement _object;
        public FrameworkElement UnderlyingObject
        {
            get
            {
                return _object;
            }

            set
            {
                _object = value;
                if (_object.ContextMenu == null)
                {
                    _object.ContextMenu = new System.Windows.Controls.ContextMenu();
                    Actions.ActionManager.PopulateMenu(_object.ContextMenu, this, new object[] { _object });
                }
            }
        }

        public FrameworkElement GetUi()
        {
            return _object;
        }
    }
}
