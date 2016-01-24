using System;
using System.Collections;
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

namespace SuperShell.Output.Viewers
{
    /// <summary>
    /// Interaction logic for NativeObjectViewer.xaml
    /// </summary>
    public partial class NativeObjectViewer : UserControl, IObjectViewer<object>
    {
        static readonly List<Type> ExtendedPrimitives = new List<Type>()
        {
            typeof(string),
            typeof(decimal),
            typeof(TimeSpan)
        };
        bool _expanded = false;
        object _obj;

        public object UnderlyingObject
        {
            get
            {
                return _obj;
            }

            set
            {
                SetObject(value, null);
            }
        }

        public NativeObjectViewer()
        {
            InitializeComponent();
            ContextMenu = new ContextMenu();
            ContextMenuOpening += NativeObjectViewer_ContextMenuOpening;
        }

        private void NativeObjectViewer_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            Actions.ActionManager.PopulateMenu(ContextMenu, this, new object[]{ UnderlyingObject});
        }

        public void SetObject(object obj, string name)
        {
            txtTitle.UnderlyingObject = obj;
            txtTitle.Text = name != null ? (name + $" ({obj})") : obj.ToString();
            this._obj = obj;
        }
        private object GetValue(dynamic p, object obj)
        {
            try
            {
                return p.GetValue(obj);
            }
            catch
            {
                return "-Error-";
            }
        }
        private void ExpandObject()
        {
            if (_obj == null||_expanded) return;
            _expanded = true;
            Type T = GetEnumerableType(_obj.GetType());
            if (T == null)
            {
                foreach (var p in _obj.GetType().GetProperties())
                {
                    AddProperty(p.Name, GetValue(p, _obj));
                }
                foreach (var p in _obj.GetType().GetFields())
                {
                    AddProperty(p.Name, GetValue(p, _obj));
                }
            }
            else
            {

                if (_obj is IDictionary)
                {
                    foreach (dynamic z in _obj as IDictionary)
                    {
                        AddProperty($"[{z.Key}]", z.Value);
                    }
                }
                else
                {
                    int ind = 0;
                    var enumerable = _obj as IEnumerable;
                    if (enumerable != null)
                    {
                        var count = enumerable.Cast<object>().Count();
                        if (count < 50)
                        {
                            foreach (var z in enumerable)
                            {
                                AddProperty($"[{ind++}]", z);
                            }
                        }
                        else
                        {
                            int beg = 0;
                            int div = (int)Math.Ceiling(count / 50.0);
                            for(int i = 0; i < 50; i++)
                            {
                                int end = Math.Min(count, beg+div);
                                AddProperty($"[{beg}-{end-1}]", enumerable.Cast<object>().Skip(beg).Take(end-beg));
                                beg = end;
                            }
                        }
                    }
                }
            }
        }
        static Type GetEnumerableType(Type type)
        {
            return (from intType in type.GetInterfaces() where intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>) select intType.GetGenericArguments()[0]).FirstOrDefault();
        }

        private void AddProperty(string name, object val)
        {
            if (val == null) val = "null";
            Type typ = val.GetType();
            if (typ.IsPrimitive || typ.IsEnum || ExtendedPrimitives.Contains(typ))
            {

                var card = new Ui.Interactive.OutputCard();
                card.RenderProperty(name, val, Ui.Interactive.OutputCard.OutputType.Normal);
                //tb.UnderlyingObject = val;
                stackPanel.Children.Add(card);
            }
            else
            {
                var pv = new NativeObjectViewer();
                pv.Margin = new Thickness(0);
                pv.SetObject(val, name);
                stackPanel.Children.Add(pv);
            }
        }
        

        public FrameworkElement GetUi()
        {
            return this;
        }

        private void expander_Collapsed(object sender, RoutedEventArgs e)
        {
            stackPanel.Height = 0;
            _expanded = false;
            stackPanel.Children.Clear();
        }

        private void expander_Expanded(object sender, RoutedEventArgs e)
        {
            stackPanel.Height = Double.NaN;
            ExpandObject();
        }
    }
}
