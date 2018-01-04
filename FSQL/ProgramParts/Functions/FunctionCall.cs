using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSQL.Interfaces;
using FSQL.ProgramParts.Core;

namespace FSQL.ProgramParts.Functions {
    public class FunctionCall : ProgramPart, IStatement, IExpression {
        public FunctionCall(string name, IEnumerable<IExpression> parameters) {
            Name = name.ToLowerInvariant();
            Parameters = parameters ?? new List<IExpression>();
        }
        public string Name { get; }

        public IEnumerable<IExpression> Parameters { get; }

        public override PartType Type { get; }
        public override void Dispose() {}
        public object Execute(IExecutionContext ctx, params object[] parms) {
            object retValue;
            var pValues = Parameters.Select(p => p.GetGenericValue(ctx)).ToArray();
            var function = ctx["#" + Name.ToLowerInvariant()] as Function;
            ctx.Trace(GetTraceString(ctx));            
            retValue = function?.InvokeGeneric(ctx, pValues);            
            return retValue;
        }

        public object GetGenericValue(IExecutionContext ctx) => Execute(ctx);

        public object InvokeGeneric(IExecutionContext ctx, params object[] parms) => Execute(ctx, parms);
        protected virtual string GetTraceString(IExecutionContext ctx) {
            if (!ctx.TracingEnabled) return null;
            var parms = Parameters.Select(p => p.GetGenericValue(ctx)).ToArray();
            var sb = new StringBuilder();
            sb.Append($"{Name}(");
            sb.Append(string.Join(", ", parms));
            sb.Append(")");
            return sb.ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{Name}(");
            sb.Append(string.Join(", ", Parameters));
            sb.Append(")");
            return sb.ToString();
        }
    }
}