using FSQL.Interfaces;

namespace FSQL.ProgramParts.Statements {
    public class TraceOn : InternalFunction {
        public TraceOn() : base(new IExpression[] {}) {}
        protected override object OnInvoke(IExecutionContext ctx, params object[] parms) => ctx.TracingEnabled = true;
        public override string ToString() => "TRACE ON;";
    }
}