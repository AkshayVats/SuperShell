using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Core
{
    internal class Evaluator:Mono.CSharp.Evaluator
    {
        public class CompilerResult
        {
            public object Result;
            public string[] Errors;
            public string[] Warnings;
        }
        private Mono.CSharp.CompilerContext _ctx;
        private static Evaluator _inst;
        public static Evaluator Inst
        {
            get
            {
                if (_inst == null)
                {
                    var ctx = new Mono.CSharp.CompilerContext(new Mono.CSharp.CompilerSettings(), new Mono.CSharp.ConsoleReportPrinter());
                    _inst = new Evaluator(ctx) { _ctx = ctx };
                }
                return _inst;
            }
        }
        private Evaluator(Mono.CSharp.CompilerContext ctx) : base(ctx) { }
        public new CompilerResult Evaluate(string code)
        {
            var errorWriter = new StringWriter();
            _ctx.Report.SetPrinter(new Mono.CSharp.StreamReportPrinter(errorWriter));
            object result;
            bool resultSet;
            try
            {
                Evaluate(code, out result, out resultSet);
                var messages = errorWriter.ToString()
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
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
    }
}
