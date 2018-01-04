using FSQL.Interfaces;
using FSQL.ProgramParts.Core;
using FSQL.ProgramParts.Functions;

namespace FSQL.ProgramParts.Statements {
    public class IfStatement : StatementBase {
        private IExpression Condition { get; }
        private CodeBlock TrueBlock { get; }
        private CodeBlock FalseBlock { get; }

        public IfStatement(IExpression condition, CodeBlock trueBlock, CodeBlock falseBlock = null) {
            Condition = condition;
            TrueBlock = trueBlock;
            FalseBlock = falseBlock;
        }

        public override void Dispose() {
            Condition.Dispose();
            TrueBlock.Dispose();
            FalseBlock?.Dispose();
        }

        protected override object OnExecute(IExecutionContext ctx, params object[] parms) {
            var condValue = IsTrue(ctx);
            return condValue ? TrueBlock.InvokeGeneric(ctx) : FalseBlock?.InvokeGeneric(ctx);
        }

        protected virtual bool IsTrue(IExecutionContext ctx) {
            var condExprValue = Condition.Execute(ctx);
            return ((condExprValue is bool && (bool)condExprValue) ||
                             (condExprValue is int && ((int)condExprValue != 0)));
        }

        public override string ToString() => $"if ({Condition}) then {TrueBlock} else {FalseBlock}";

        protected override string GetTraceMessage(IExecutionContext ctx) {
            if (!ctx.TracingEnabled) return null;
            var isTrue = IsTrue(ctx);
            var path = isTrue ? TrueBlock : FalseBlock;
            return $"EVAL:: IF ({Condition}) => {(isTrue?"TRUE":"FALSE")}";
        }
    }
}