using FSQL.Interfaces;
using FSQL.ProgramParts.Core;
using FSQL.ProgramParts.DataTypes;
using FSQL.ProgramParts.Functions;

namespace FSQL.ProgramParts.Statements {
    public class WhileStatement : StatementBase
    {
        private IExpression Condition { get; }
        private CodeBlock Body { get; }

        public WhileStatement(IExpression condition, CodeBlock body)
        {
            Condition = condition;
            Body = body;
        }

        public override void Dispose()
        {
            Condition.Dispose();
            Body.Dispose();
        }

        protected override object OnExecute(IExecutionContext ctx, params object[] parms)
        {
            var loops = 0;
            while (CanContinue(ctx)) {
                loops++;
                Body.InvokeGeneric(ctx);
            }
            return (IntConstant) loops;
        }

        protected bool CanContinue(IExecutionContext ctx) {
            var condExprValue = Condition.Execute(ctx);
            return ((condExprValue is bool && (bool)condExprValue) ||
                    (condExprValue is int && ((int)condExprValue != 0)));
        }

        public override string ToString() => $"while /{Condition}/ do /{Body}/;";
    }
}