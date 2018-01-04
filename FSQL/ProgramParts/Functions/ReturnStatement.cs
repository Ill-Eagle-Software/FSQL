using FSQL.Interfaces;
using FSQL.ProgramParts.Core;

namespace FSQL.ProgramParts.Functions {
    public class ReturnStatement : StatementBase {
        private IExpression _returnValueExpr;

        public ReturnStatement(IExpression returnValueExpr) {
            _returnValueExpr = returnValueExpr;
        }

        public ReturnStatement(IProgramPart returnValueExpr) : this(returnValueExpr as IExpression) {}

        public override void Dispose() {
            _returnValueExpr.Dispose();
            _returnValueExpr = null;
        }

        protected override object OnExecute(IExecutionContext ctx, params object[] parms)
        {
            //var returnVal = _returnValueExpr.GetGenericValue(ctx);
            var retval = _returnValueExpr.GetGenericValue(ctx);
            ctx.SetReturnValue(retval);
            return retval;
        }

        public override string ToString() => $"return {_returnValueExpr};";
    }
}