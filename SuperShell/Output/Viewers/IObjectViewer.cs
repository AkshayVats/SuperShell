using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SuperShell.Output.Viewers
{
    public interface IObjectViewer
    {
        FrameworkElement GetUi();
    }
    public interface IObjectViewer<T> : IObjectViewer
    {
        T UnderlyingObject { get; set; }
    }
}
