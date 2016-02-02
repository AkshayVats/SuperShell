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
using SuperShell.Core;
using SuperShell.Util;

namespace SuperShell.Output.Viewers
{
    /// <summary>
    /// Interaction logic for OutputWithType.xaml
    /// </summary>
    public partial class OutputWithType : UserControl, IObjectViewer<object>
    {
        Type _outputType;
        object _underlyingObject;
        
        public string Text
        {
            get
            {
                return txtContent.Text;
            }
            set
            {
                txtContent.Text = value;
            }
        }
        
        public Type OutputType { get { return _outputType; }
            set
            {
                _outputType = value;
                txtType.Inlines.Clear();
                txtType.Inlines.Add(new Italic(new Run(_outputType.GetStringRepresentation() )) );
                txtType.ToolTip = value;
            }
        }

        public object UnderlyingObject
        {
            get
            {
                return _underlyingObject;
            }

            set
            {
                _underlyingObject = value;
                Text = _underlyingObject.ToString();
                OutputType = _underlyingObject.GetType();
            }
        }

        public OutputWithType()
        {
            InitializeComponent();
        }
        

        public FrameworkElement GetUi()
        {
            return this;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            txtType.MaxWidth = RenderSize.Width * 0.2;
        }
    }
}
