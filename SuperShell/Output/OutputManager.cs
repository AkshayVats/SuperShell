using System;
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
                    return new Ui.OutputWithType() { Text=obj.ToString(), OutputType=obj.GetType() };
                }
            }

            return null;
        }
    }
}
