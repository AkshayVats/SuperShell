using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SuperShell.Output.Viewers
{
    public class ImageViewer : IObjectViewer<ImageSource>
    {
        private Image _image = new Image();
        public ImageSource UnderlyingObject
        {
            get
            {
                return _image.Source;
            }

            set
            {
                _image.Source = value;
            }
        }

        public FrameworkElement GetUi()
        {
            return _image;
        }
    }
}
