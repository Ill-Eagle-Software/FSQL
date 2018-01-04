using System.Collections.Generic;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.Statements {
    public class Trace : InternalFunction {
        public Trace(IEnumerable<IExpression> parameters) : base(parameters) {}
        protected override object OnInvoke(IExecutionContext ctx, params object[] parms) {
            return "Not Implemented.";
        }
    }
}