using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FSQL.Interfaces;
using FSQL.ProgramParts.Core;

namespace FSQL.ProgramParts.Functions {
    public class Function : ProgramPart, IStatement {
        public Function(string name, IEnumerable<string> parameterNames, IStatement body) {
            Name = name;
            ParameterNames = parameterNames ?? new List<string>();
            Body = body;
        }

        public string Name { get; }

        public IEnumerable<string> ParameterNames { get; }

        protected IStatement Body { get; }

        public override PartType Type { get; } = PartType.Function;
        public override void Dispose() {}

        public object Execute(IExecutionContext ctx, params object[] parms) {
            ctx["#" + Name.ToLowerInvariant()] = this;
            return $"(function {Name}) ";
        }

        protected void AddParameters(IExecutionContext ctx, object[] parms) {
            if (ParameterNames.Count() != parms.Count())
                throw new TargetParameterCountException($"Param count mismatch in {Name}.");
            var pIdx = 0;
            foreach (var name in ParameterNames) {
                ctx[name] = parms[pIdx++];
            }
        }

        public object InvokeGeneric(IExecutionContext ctx, params object[] parms) {
            object results = null;
            var myContext = ctx.Enter(Name);
            try {
                AddParameters(myContext, parms);
                results = Body.InvokeGeneric(myContext, parms);
            } finally {
                myContext.Exit(results);
            }
            return results;
        }

        public string GetListing() {
            var sb = new StringBuilder();
            sb.Append(ToString());
            
            var body = Body as CodeBlock;
            if (body != null) {
                sb.Append(body.GetListing());
            } else sb.AppendLine("{}");
            
            return sb.ToString();
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append($"FUNC {Name}(");
            sb.Append(string.Join(", ", ParameterNames.Select(pn => "@"+pn)));
            sb.Append(")");
            return sb.ToString();
        }
    }
}