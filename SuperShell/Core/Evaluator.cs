using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SuperShell.Bridge.Core;
using SuperShell.Util;

namespace SuperShell.Core
{
    public class Evaluator:Mono.CSharp.Evaluator, Bridge.Core.IShell
    {
        public class CompilerResult
        {
            public object Result;
            public string[] Errors;
            public string[] Warnings;
        }

        private static Evaluator _inst;
        private static object _lock = new object();


        private Mono.CSharp.CompilerContext _ctx;
        private List<string> _assemblyPaths = new List<string>();
        internal static void NewInstance()
        {
            var ctx = new Mono.CSharp.CompilerContext(new Mono.CSharp.CompilerSettings(), new Mono.CSharp.ConsoleReportPrinter());
            _inst = new Evaluator(ctx);
        }
        public static Evaluator Inst
        {
            get
            {
                return _inst;
            }
        }

        public string[] LoadedAssemblyLocations
        {
            get
            {
                return _assemblyPaths.ToArray();
            }
        }
        private Dictionary<string, string> compiledDocuments = new Dictionary<string, string>();
        public Document[] CompiledDocuments
        {
            get
            {
                return compiledDocuments.Select(i=>new Document(i.Key, i.Value)).ToArray();
            }
        }

        private Evaluator(Mono.CSharp.CompilerContext ctx) : base(ctx)
        {
            _ctx = ctx;
        }
        public new CompilerResult Evaluate(string code)
        {
            var errorWriter = new StringWriter();
            _ctx.Report.SetPrinter(new Mono.CSharp.StreamReportPrinter(errorWriter));
            object result;
            bool resultSet;
            try
            {
                //Console.WriteLine($"Evaluating '{code}'");
                Evaluate(code, out result, out resultSet);
                var messages = errorWriter.ToString()
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine(errorWriter.ToString());
                return new CompilerResult
                {
                    Result = result,
                    Errors = messages.Where(msg => IsMessage(msg, "error")).ToArray(),
                    Warnings = messages.Where(msg => IsMessage(msg, "warning")).ToArray()
                };
            }
            catch (Exception ex)
            {
                return new CompilerResult()
                {
                    Errors = new string[] { ex.Message }
                };
            }
        }
        
        private static bool IsMessage(string message, string messageType)
        {
            //the messages have following format
            // (2,1): error (123): Bla bla bla
            var messageParts = message.Split(':');
            if (messageParts.Length >= 2)
            {
                var type = messageParts[1].Trim();
                return type.StartsWith(messageType, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public static object TmpVar;
        int varCount;

        public event EventHandler<Assembly> AssemblyReferenced;
        public event EventHandler<Document> DocumentCompiled;

        public new void ReferenceAssembly(Assembly assembly)
        {
            base.ReferenceAssembly(assembly);
            _assemblyPaths.Add(assembly.Location);
            AssemblyReferenced?.Invoke(this, assembly);
        }
        public new void LoadAssembly(string path)
        {
            if (_assemblyPaths.Contains(path)) return;
            var assembly = AppDomain.CurrentDomain.GetAssemblies().Where(i=>i.IsDynamic==false).FirstOrDefault(i => i.Location == path);
            if(assembly == null)
            {
                //Console.WriteLine("Loaded a new assembly, "+path);
                assembly = Assembly.LoadFile(path);
            }
            ReferenceAssembly(assembly);
        }
        public string RefrenceObject(object obj)
        {
            TmpVar = obj;
            var varName = "var" + varCount;
            var msg = Evaluate($"var {varName}=({obj.GetType().GetStringRepresentation()})SuperShell.Core.Evaluator.TmpVar;");
            if(msg.Errors.Length>0)
                Evaluate($"var {varName}=SuperShell.Core.Evaluator.TmpVar;");
            varCount++;
            return varName;
        }
        public new string GetVars()
        {
            var vars = base.GetVars().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join("\n", vars.Select(i => i.Split('=')[0]+";"));
        }
        public void CompileDocument(string text, string path)
        {
            var errorWriter = new StringWriter();
            _ctx.Report.SetPrinter(new Mono.CSharp.StreamReportPrinter(errorWriter));

            Mono.CSharp.CompiledMethod mth;
            try {
                var zz = Compile(text, out mth);
            }catch(Exception ex)
            {

            }
            Console.WriteLine(errorWriter.ToString());

            compiledDocuments[path] = text;
            DocumentCompiled?.Invoke(this, new Document(path, text));
        }
    }
}
