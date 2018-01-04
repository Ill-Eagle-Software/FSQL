using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSQL.Interfaces;
using FSQL.ProgramParts.Core;

namespace FSQL.ProgramParts.Functions {
    public class CodeBlock : StatementBase {

        protected string Name { get; }
        public CodeBlock(string name, IEnumerable<IStatement> body) {
            Name = name;
            Body = body;
        }

        private IEnumerable<IStatement> Body { get; }

        protected override object OnExecute(IExecutionContext ctx, params object[] parms)
        {
            object results = null;
            //var myContext = ctx.Enter(Name);
            try {
                foreach (var stmt in Body) {
                    try {
                        results = stmt.Execute(ctx, parms);
                    } catch (Exception ex) {
                        if (ctx.Throw(ex, $"Exception thrown by statement [{stmt}]")) throw;
                    }
                    if (stmt is ReturnStatement) {
                        break;
                    }
                }
            } finally {
                //ctx.Exit(results);
            }
            return results;
        }

        public override void Dispose() { }

        public virtual string GetListing() {
            var sb = new StringBuilder();            
            sb.AppendLine("{");
            foreach (var ln in Body)
            {
                sb.AppendLine($"   {ln}");
            }            
            sb.AppendLine("}");            
            return sb.ToString();
        }

        public override string ToString() {                
            var sb = new StringBuilder();
            //sb.AppendLine($"BEGIN CODEBLOCK: {Name}");
            sb.AppendLine("{");
            foreach (var ln in Body.Take(3)) {
                sb.AppendLine($"   {ln}");
            }
            if (Body.Count() > 3) sb.AppendLine($"   ...");
            sb.AppendLine("}");
            //sb.AppendLine($"  END CODEBLOCK: {Name}");
            return sb.ToString();
        }

        protected override string GetTraceMessage(IExecutionContext ctx) {
            return null;
        }
    }
}