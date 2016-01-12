using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SuperShell.Ui
{
    /// <summary>
    /// Interaction logic for OutputCard.xaml
    /// </summary>
    public partial class OutputCard : UserControl
    {
        public OutputCard()
        {
            InitializeComponent();
        }

        internal enum OutputType { Error, Normal, Gray }

        internal void Render(object obj, OutputType typ)
        {
            var output = Output.OutputManager.GenerateOutput(obj);
            if (!(output is Output.Viewers.OutputWithType))
                typ = OutputType.Gray;
            Content = output;
            Style = GetStyle(typ);
            if ((output as Control) != null)
                (output as Control).Foreground = Foreground;
        }
        internal void RenderMessage(string msg, OutputType typ)
        {
            var output = Output.OutputManager.GenerateMessage(msg);
            Content = output;
            Style = GetStyle(typ);
            if ((output as Control) != null)
                (output as Control).Foreground = Foreground;
        }
        internal void RenderProperty(string propertyName, object value, OutputType typ)
        {
            var output = new TextBlock()
            {
                Padding = new Thickness(2),
                Inlines = {
                    new Bold(new Run(propertyName) { Foreground = new SolidColorBrush(Colors.Red) }),
                    new Run(" : " + value.ToString()) { Foreground = new SolidColorBrush(Colors.Black) }
                    }
            };
            Content = output;
            Style = GetStyle(typ);
            
        }

        private Style GetStyle(OutputType typ)
        {
            switch (typ)
            {
                case OutputType.Error: return FindResource("ErrorStyle") as Style;
                case OutputType.Normal: return FindResource("NormalOutputStyle") as Style;
                case OutputType.Gray:return FindResource("GrayOutputStyle") as Style;
            }
            return null;
        }
    }
}
