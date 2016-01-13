using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SuperShell.Output
{
    static class OutputManager
    {
        public static FrameworkElement GenerateMessage(string msg)
        {
            return new Ui.ShellSelectableText() { Text = msg };
        }
        public static FrameworkElement GenerateOutput(object obj)
        {
            if (obj != null)
            {
                if (obj.GetType().IsPrimitive || obj is string)
                {
                    return new Viewers.OutputWithType() { UnderlyingObject = obj };
                }
                else if (obj is FrameworkElement) return new Viewers.FrameworkElementViewer() {UnderlyingObject= (obj as FrameworkElement) }.GetUi();
                return new Viewers.NativeObjectViewer() { UnderlyingObject = obj }.GetUi();
            }
            
            return null;
        }

        static Dictionary<Type, List<Type>> ObjectViews = new Dictionary<Type, List<Type>>()
        {
            { typeof(object), new List<Type>() {typeof(Viewers.OutputWithType), typeof(Viewers.NativeObjectViewer) } },
            {typeof(IEnumerable), new List<Type>() {typeof(Viewers.ListViewer) } },
            {typeof(ImageSource), new List<Type>() {typeof(Viewers.ImageViewer) } }
        };
        public static IEnumerable<Type> GetViewersFor(Type typ)
        {
            return ObjectViews.Where(i => i.Key.IsAssignableFrom(typ)).SelectMany(i => i.Value);
        }
    }
}
