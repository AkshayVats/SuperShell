using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SuperShell.Output.Viewers
{
    class AssemblyViewer:IObjectViewer<System.Reflection.Assembly>
    {
        public Assembly UnderlyingObject
        {
            get; set;
        }

        class NamespaceNode
        {
            public string Name;
            public Dictionary<string, NamespaceNode> Children = new Dictionary<string, NamespaceNode>();
            public List<Type> Types = new List<Type>();
            public override string ToString()
            {
                return Name;
            }
        }
        
        private void Expander(INOV nov, object obj)
        {
            var node = obj as NamespaceNode;
            if (node == null)
            {
                nov.AddProperty("Error", "obj is not NamespaceNode");
                return;
            }

            foreach (var n in node.Children)
                nov.AddProperty("{"+n.Key+"}", n.Value);
            foreach (var t in node.Types)
                nov.AddProperty(t.Name, t);
        }
        private void ExpanderForType(INOV nov, object obj)
        {
            var node = obj as Type;
            if (node == null)
            {
                nov.AddProperty("Error", "obj is not Type");
                return;
            }

            foreach (var n in node.GetFields())
                nov.AddProperty("[Field]", n.FieldType.Name+" "+ n.Name);
            foreach (var t in node.GetProperties())
                nov.AddProperty("[Property]",t.PropertyType.Name+" "+ t.Name);
            foreach (var t in node.GetMethods())
                nov.AddProperty("[m]", t.ReturnType+" "+ t.Name+"("+string.Join(", ", t.GetParameters().Select(i=>i.ParameterType.Name+" "+ i.Name))+")");
        }
        private NamespaceNode ClassifyTypes(System.Reflection.Assembly assembly)
        {
            if (!NativeObjectViewer.HasExpander(typeof(NamespaceNode)))
            {
                NativeObjectViewer.AddExpander(typeof(NamespaceNode), Expander);
                NativeObjectViewer.AddExpander(this.GetType().GetType(), ExpanderForType);
            }
            var types = assembly.GetTypes();
            NamespaceNode root = new NamespaceNode() { Name = "{Root}" };
            foreach (var typ in types)
            {
                NamespaceNode node = root;
                if (typ.Namespace != null)
                foreach (var ns in typ.Namespace.Split('.'))
                {
                    if (!node.Children.ContainsKey(ns))
                        node.Children[ns] = new NamespaceNode() { Name = ns };
                    node = node.Children[ns];
                }
                node.Types.Add(typ);
            }
            return root;
        }
        
        public FrameworkElement GetUi()
        {
            var ov = new NativeObjectViewer();
            ov.UnderlyingObject = ClassifyTypes(UnderlyingObject);
            return ov;
        }
    }
}
