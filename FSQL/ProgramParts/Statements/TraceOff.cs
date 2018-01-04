using FSQL.Interfaces;

namespace FSQL.ProgramParts.Statements {
    public class TraceOff : InternalFunction {
        public TraceOff() : base(new IExpression[] { }) { }
        protected override object OnInvoke(IExecutionContext ctx, params object[] parms) => ctx.TracingEnabled = false;
        public override string ToString() => "TRACE OFF;";
    }
}