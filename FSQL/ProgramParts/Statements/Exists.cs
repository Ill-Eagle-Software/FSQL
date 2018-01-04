using FSQL.Interfaces;

namespace FSQL.ProgramParts.Statements {
    public class Exists : InternalFunction
    {
        private readonly string _varName;
        public Exists(string varName) : base(new IExpression[]{}) {
#if VARIABLES_ARE_not_CASE_SENSITIVE
            _varName = varName.ToLowerInvariant();
#else
          _varName = varName;
#endif
        }
        protected override object OnInvoke(IExecutionContext ctx, params object[] parms) {            
            return ctx.HasVariable(_varName);
        }

        
    }
}