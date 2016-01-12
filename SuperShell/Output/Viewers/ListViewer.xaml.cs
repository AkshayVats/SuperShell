using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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

namespace SuperShell.Output.Viewers
{
    /// <summary>
    /// Interaction logic for ListViewer.xaml
    /// </summary>
    public partial class ListViewer : UserControl,IObjectViewer<IEnumerable>
    {
        public ListViewer()
        {
            InitializeComponent();
        }
        IEnumerable _underlyingObject;
        public IEnumerable UnderlyingObject
        {
            get
            {
                return _underlyingObject;
            }

            set
            {
                SetObject(value);
            }
        }

        public FrameworkElement GetUi()
        {
            return this;
        }

        private void SetObject(IEnumerable obj)
        {
            _underlyingObject = obj;
            //Try to figure out enumerable generic parameter
            var typ = obj.GetType().GetElementType(); //easy way
            if (typ == null)
            {
                if (obj.GetType() == typeof(StringCollection))
                    typ = typeof(string);
                else typ = (from intType in obj.GetType().GetInterfaces() where intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>) select intType.GetGenericArguments()[0]).FirstOrDefault();
            }
            //typ may be null...fuck it get exception

            var grid = listView.View as GridView;
            foreach (var t in typ.GetProperties())
            {
                var gvc = new GridViewColumn()
                {
                    Header = string.Concat(t.Name.Select(c => Char.IsUpper(c) ? " " + c : c.ToString())).TrimStart(' '),

                };
                if (typeof(BitmapImage).IsAssignableFrom(t.PropertyType))
                {
                    var fac = new FrameworkElementFactory(typeof(Image));
                    fac.SetBinding(Image.SourceProperty, new Binding(t.Name));
                    fac.SetValue(Image.MaxHeightProperty, 100.0);
                    gvc.CellTemplate = new DataTemplate(typeof(BitmapImage))
                    {
                        VisualTree = fac
                    };
                }
                else gvc.DisplayMemberBinding = new Binding(t.Name);

                grid.Columns.Add(gvc);
            }
            foreach (var t in typ.GetFields())
            {
                grid.Columns.Add(new GridViewColumn()
                {
                    Header = string.Concat(t.Name.Select(c => Char.IsUpper(c) ? " " + c : c.ToString())).TrimStart(' '),
                    DisplayMemberBinding = new Binding(t.Name)
                });
            }
            BindingOperations.EnableCollectionSynchronization(obj, new object());
            listView.ItemsSource = obj;
        }
    }
}
