using FSQL.Interfaces;

namespace FSQL.ProgramParts.Variables {
    public class VariableSet : VariableBase, IStatement {
        private IExpression Expression { get; }
        public VariableSet(string name, IExpression value) : base(name)
        {
            Expression = value;
        }

        public override PartType Type { get; } = PartType.Statement;

        public override dynamic GetGenericValue(IExecutionContext ctx) {
            var value = Expression.GetGenericValue(ctx);
            ctx.Trace(GetTraceString(ctx));
            ctx[Name] = value;
            return value;
        }

        public object InvokeGeneric(IExecutionContext ctx, params object[] parms) =>
            GetGenericValue(ctx);

        public override string ToString() => $"@{Name} = {Expression};";

        protected override string GetTraceString(IExecutionContext ctx) {
            if (!ctx.TracingEnabled) return null;
            return $"@{Name} = {Expression.GetGenericValue(ctx)};";
        }
    }
}