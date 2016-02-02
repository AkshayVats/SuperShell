using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SuperShell.Util
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
        public static LinkedListNode<string> NextOrFirst(this LinkedListNode<string> current)
        {
            return current.Next?? current.List.First;
        }

        public static LinkedListNode<string> PreviousOrLast(this LinkedListNode<string> current)
        {
            return current.Previous?? current.List.Last;
        }


        #region -Webbrowser-
        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetCookieEx(
        string url,
        string cookieName,
        StringBuilder cookieData,
        ref int size,
        Int32 dwFlags,
        IntPtr lpReserved);

        private const Int32 InternetCookieHttponly = 0x2000;

        public static CookieContainer GetUriCookieContainer(this WebBrowser wb)
        {
            var uri = wb.Source;
            CookieContainer cookies = null;
            // Determine the size of the cookie
            int datasize = 8192 * 16;
            StringBuilder cookieData = new StringBuilder(datasize);
            if (!InternetGetCookieEx(uri.ToString(), null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                // Allocate stringbuilder large enough to hold the cookie
                cookieData = new StringBuilder(datasize);
                if (!InternetGetCookieEx(
                    uri.ToString(),
                    null, cookieData,
                    ref datasize,
                    InternetCookieHttponly,
                    IntPtr.Zero))
                    return null;
            }
            if (cookieData.Length > 0)
            {
                cookies = new CookieContainer();
                cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
            }
            return cookies;
        }
        #endregion
    }
}
