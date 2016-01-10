using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SuperShell.Ui
{
    class ShellSelectableText:TextBox
    {
        public ShellSelectableText()
        {
            IsReadOnly = true;
            Style = FindResource("TB_no_border") as Style;
        }
    }
}
