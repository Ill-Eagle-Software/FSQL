using FSQL.Interfaces;

namespace FSQL.ProgramParts.Core {
    public abstract class StatementBase : ProgramPart, IStatement {
        public sealed override PartType Type { get; } = PartType.Statement;

        public object InvokeGeneric(IExecutionContext ctx, params object[] parms) {
            ctx.Trace(GetTraceMessage(ctx));
            return OnExecute(ctx, parms);
        }

        object IExecutable.Execute(IExecutionContext ctx, params object[] parms) => InvokeGeneric(ctx, parms);

        protected abstract object OnExecute(IExecutionContext ctx, params object[] parms);

        protected virtual string GetTraceMessage(IExecutionContext ctx) => ToString();
    }
}