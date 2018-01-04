using FSQL.Interfaces;
using FSQL.ProgramParts.Core;
using FSQL.ProgramParts.Functions;

namespace FSQL.ProgramParts.Statements {
    public class Simulate : StatementBase, IExpression {
        private readonly CodeBlock _block;

        public Simulate(CodeBlock block) {
            _block = block;
        }

        protected override object OnExecute(IExecutionContext ctx, params object[] parms) {
            var simCtx = ctx.Enter("__Simulation__");
            object returnValue = null;
            try {
                simCtx.InSimulationMode = true;
                returnValue = _block.InvokeGeneric(simCtx, parms);
            } finally {
                simCtx.Exit(returnValue);
            }
            return returnValue;
        }

        public override void Dispose() {
            _block.Dispose();
        }

        object IExpression.GetGenericValue(IExecutionContext ctx) => OnExecute(ctx);

        public override string ToString() {
            return "simulate" + _block;
        }
    }
}