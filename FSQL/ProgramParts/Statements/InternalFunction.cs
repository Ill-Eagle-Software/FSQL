using System.Collections.Generic;
using System.Linq;
using FSQL.Interfaces;
using FSQL.ProgramParts.Core;

namespace FSQL.ProgramParts.Statements {
    public abstract class InternalFunction : StatementBase, IExpression {
        protected InternalFunction(IEnumerable<IExpression> parameters) {
            Parameters = parameters ?? new IExpression[] {};
        }


        public IEnumerable<IExpression> Parameters { get; }

        protected override object OnExecute(IExecutionContext ctx, params object[] parms)
        {
            var pValues = Parameters.Select(p => p.GetGenericValue(ctx)).ToArray();
            return OnInvoke(ctx, pValues);
        }

        protected abstract object OnInvoke(IExecutionContext ctx, params object[] parms);
        public override void Dispose() { }
        public object GetGenericValue(IExecutionContext ctx) => InvokeGeneric(ctx);
    }
}