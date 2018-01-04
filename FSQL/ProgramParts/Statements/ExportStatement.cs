using System.Linq;
using System.Text;
using FSQL.Interfaces;
using FSQL.ProgramParts.Core;

namespace FSQL.ProgramParts.Statements {
    public class ExportStatement : StatementBase
    {
        private ProgramTerminalList<string> Parameters;

        public ExportStatement(ProgramTerminalList<string> parms)
        {
            Parameters = parms;
        }

        public override void Dispose()
        {
            Parameters.Dispose();
            Parameters = null;
        }

        protected override object OnExecute(IExecutionContext ctx, params object[] parms)
        {
            //var returnVal = _returnValueExpr.GetGenericValue(ctx);
            foreach (var p in Parameters) {
                ctx.Export(p.Trim("@".ToCharArray()));
            }
            return null;
        }

        public override string ToString() {
            if (Parameters.Count == 1) return $"export @{Parameters.First()};";
            var sb = new StringBuilder("export ");
            foreach (var p in Parameters.Take(Parameters.Count-1)) {
                sb.Append($"@{p}, ");
            }
            sb.Append($"{Parameters.Last()};");
            return sb.ToString();
        }
    }
}