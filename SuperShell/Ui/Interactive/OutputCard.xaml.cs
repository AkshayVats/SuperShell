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

namespace SuperShell.Ui.Interactive
{
    /// <summary>
    /// Interaction logic for OutputCard.xaml
    /// </summary>
    public partial class OutputCard : UserControl
    {
        object _underlyingObject;
        public event EventHandler Crossed;
        public OutputCard()
        {
            InitializeComponent();
        }

        internal enum OutputType { Error, Normal, Gray }

        internal void Render(object obj, OutputType typ)
        {
            _underlyingObject = obj;
            var output = Output.OutputManager.GenerateOutput(obj);
            if (!(output is Output.Viewers.OutputWithType))
                typ = OutputType.Gray;
            ChooseViewers(obj.GetType());
            SetOutput(output);
            Style = GetStyle(typ);
            if ((output as Control) != null)
                (output as Control).Foreground = Foreground;
        }

        private void ChooseViewers(Type type)
        {
            if (type.IsPrimitive) return;
            cbViewers.ItemsSource =  Output.OutputManager.GetViewersFor(type);
            if (cbViewers.Items.Count > 1) cbViewers.Visibility = Visibility.Visible;
        }

        private void SetOutput(FrameworkElement output)
        {
            panel.Children.Clear();
            panel.Children.Add(output);
        }

        internal void RenderMessage(string msg, OutputType typ)
        {
            var output = Output.OutputManager.GenerateMessage(msg);
            SetOutput(output);
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
            SetOutput(output);
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

        private void cbViewers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO: get rid of dynamic, does not support explicit interfaces
            dynamic output = Activator.CreateInstance(cbViewers.SelectedItem as Type);
            output.UnderlyingObject = ((dynamic)_underlyingObject);
            SetOutput(output.GetUi());
        }

        private void CrossButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Crossed?.Invoke(this, null);
        }
    }
}
