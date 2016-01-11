using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Core
{
    public static class Extensions
    {
        public static string GetStringRepresentation(this Type type)
        {
            StringBuilder retType = new StringBuilder();
            if (type.IsGenericType)
            {
                string[] parentType = type.FullName.Split('`');
                string enumerable = parentType[0];
                enumerable = enumerable.Replace('+', '.');
                // We will build the type here.
                Type[] arguments = type.GetGenericArguments();

                StringBuilder argList = new StringBuilder();
                foreach (Type t in arguments)
                {
                    // Let's make sure we get the argument list.
                    string arg = t.GetStringRepresentation();
                    if (argList.Length > 0)
                    {
                        argList.AppendFormat(", {0}", arg);
                    }
                    else
                    {
                        argList.Append(arg);
                    }
                }

                if (argList.Length > 0)
                {
                    retType.AppendFormat("{0}<{1}>", enumerable, argList.ToString());
                }
            }
            else
            {
                return type.ToString();
            }

            return retType.ToString();
        }
    }
}
