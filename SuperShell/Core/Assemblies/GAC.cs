using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Core.Assemblies
{
    public class GAC
    {
        #region -Native Stuff-
        [ComImport(), Guid("CD193BC0-B4BC-11D2-9833-00C04FC31D2E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAssemblyName
        {
            //
            // Assembly name properties
            //  0 = PublicKey, byte[]*          ; ???
            //  1 = PublicKeyToken, byte[8]*
            //  3 = Assembly Name, LPWSTR
            //  4 = Major Version, ushort*
            //  5 = Minor Version, ushort*
            //  6 = Build Number, ushort*
            //  7 = Revison Number, ushort*
            //  8 = Culture, LPWSTR
            //  9 = Processor Type, ???         ; ???
            // 10 = OS Type, ???                ; ???
            // 13 = Codebase, LPWSTR
            // 14 = Modified Date, FILETIME*    ; Only for Downloaded assemblies ?
            // 17 = Custom, LPWSTR              ; ZAP string, only for NGEN assemblies
            // 19 = MVID, byte[16]*             ; MVID value from __AssemblyInfo__.ini - what's this?
            //
            [PreserveSig()]
            int Set(uint PropertyId,
                    IntPtr pvProperty,
                    uint cbProperty);

            [PreserveSig()]
            int Get(uint PropertyId,
                    IntPtr pvProperty,
                    ref uint pcbProperty);

            [PreserveSig()]
            int Finalize();

            [PreserveSig()]
            int GetDisplayName([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder szDisplayName,
                               ref uint pccDisplayName,
                               uint dwDisplayFlags);

            [PreserveSig()]
            int BindToObject(object refIID,
                             object pAsmBindSink,
                             IApplicationContext pApplicationContext,
                             [MarshalAs(UnmanagedType.LPWStr)] string szCodeBase,
                             long llFlags,
                             int pvReserved,
                             uint cbReserved,
                             out int ppv);

            [PreserveSig()]
            int GetName(ref uint lpcwBuffer,
                        [Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwzName);

            [PreserveSig()]
            int GetVersion(out uint pdwVersionHi,
                           out uint pdwVersionLow);

            [PreserveSig()]
            int IsEqual(IAssemblyName pName,
                        uint dwCmpFlags);

            [PreserveSig()]
            int Clone(out IAssemblyName pName);
        }
        [ComImport(), Guid("7C23FF90-33AF-11D3-95DA-00A024A85B51"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IApplicationContext
        {
            void SetContextNameObject(IAssemblyName pName);

            void GetContextNameObject(out IAssemblyName ppName);

            void Set([MarshalAs(UnmanagedType.LPWStr)] string szName,
                     int pvValue,
                     uint cbValue,
                     uint dwFlags);

            void Get([MarshalAs(UnmanagedType.LPWStr)] string szName,
                     out int pvValue,
                     ref uint pcbValue,
                     uint dwFlags);

            void GetDynamicDirectory(out int wzDynamicDir,
                                     ref uint pdwSize);
        }
        [ComImport(), Guid("21B8916C-F28E-11D2-A473-00C04F8EF448"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAssemblyEnum
        {
            [PreserveSig()]
            int GetNextAssembly(out IApplicationContext ppAppCtx,
                                out IAssemblyName ppName,
                                uint dwFlags);

            [PreserveSig()]
            int Reset();

            [PreserveSig()]
            int Clone(out IAssemblyEnum ppEnum);
        }

        [DllImport("fusion.dll", CharSet = CharSet.Auto)]
        internal static extern int CreateAssemblyEnum(out IAssemblyEnum ppEnum,
                                                      IApplicationContext pAppCtx,
                                                      IAssemblyName pName,
                                                      uint dwFlags,
                                                      int pvReserved);
        #endregion

        public static IEnumerable<AssemblyName> GetAssemblyList()
        {
            IApplicationContext applicationContext = null;
            IAssemblyEnum assemblyEnum = null;
            IAssemblyName assemblyName = null;

            CreateAssemblyEnum(out assemblyEnum, null, null, 2, 0);
            while (assemblyEnum.GetNextAssembly(out applicationContext, out assemblyName, 0) == 0)
            {
                uint nChars = 0;
                assemblyName.GetDisplayName(null, ref nChars, 0);

                StringBuilder sb = new StringBuilder((int)nChars);
                assemblyName.GetDisplayName(sb, ref nChars, 0);

                yield return new AssemblyName(sb.ToString());
            }
            
        }

        [DllImport("fusion.dll")]
        internal static extern int GetCachePath(uint flags,
                                                [MarshalAs(UnmanagedType.LPWStr)] StringBuilder wzDir,
                                                ref uint pdwSize);
        public static string GetGacPath(bool isCLRv4 = false)
        {
            const uint ASM_CACHE_ROOT = 0x08; // CLR V2.0
            const uint ASM_CACHE_ROOT_EX = 0x80; // CLR V4.0
            uint flags = isCLRv4 ? ASM_CACHE_ROOT_EX : ASM_CACHE_ROOT;

            const int size = 260; // MAX_PATH
            StringBuilder b = new StringBuilder(size);
            uint tmp = size;
            GetCachePath(flags, b, ref tmp);
            return b.ToString();
        }

        static readonly string cachedGacPathV2 = GetGacPath(false);
        static readonly string cachedGacPathV4 = GetGacPath(true);

        public static string GacRootPathV2
        {
            get { return cachedGacPathV2; }
        }

        public static string GacRootPathV4
        {
            get { return cachedGacPathV4; }
        }

        static readonly string[] gac_paths = { GacRootPathV2, GacRootPathV4 };
        static readonly string[] gacs = { "GAC_MSIL", "GAC_32", "GAC" };
        static readonly string[] prefixes = { string.Empty, "v4.0_" };
        /// <summary>
        /// Gets the file name for an assembly stored in the GAC.
        /// </summary>
        public static string FindAssemblyInNetGac(AssemblyName reference)
        {
            // without public key, it can't be in the GAC
            if (reference.GetPublicKeyToken() == null || reference.GetPublicKeyToken().Length == 0)
                return null;

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < gacs.Length; j++)
                {
                    var gac = Path.Combine(gac_paths[i], gacs[j]);
                    var file = GetAssemblyFile(reference, prefixes[i], gac);
                    if (File.Exists(file))
                        return file;
                }
            }

            return null;
        }
        static string GetAssemblyFile(AssemblyName reference, string prefix, string gac)
        {
            var gac_folder = new StringBuilder()
                .Append(prefix)
                .Append(reference.Version)
                .Append("__");

            gac_folder.Append(PublicKeyTokenToString(reference));

            return Path.Combine(
                Path.Combine(
                    Path.Combine(gac, reference.Name), gac_folder.ToString()),
                reference.Name + ".dll");
        }

        //example from here: http://msdn.microsoft.com/en-us/library/system.reflection.assemblyname.getpublickeytoken(v=vs.95).aspx
        private const byte mask = 15;
        private const string hex = "0123456789ABCDEF";

        public static string PublicKeyTokenToString(AssemblyName assemblyName)
        {
            var pkt = new System.Text.StringBuilder();
            if (assemblyName.GetPublicKeyToken() == null)
                return String.Empty;

            foreach (byte b in assemblyName.GetPublicKeyToken())
            {
                pkt.Append(hex[b / 16 & mask]);
                pkt.Append(hex[b & mask]);
            }
            return pkt.ToString();
        }
    }
}
