using FSQL.Interfaces;

namespace FSQL.ProgramParts.Variables {
    public abstract class VariableBase : IExpression {

        public string Name { get; }
        protected VariableBase(string name) {
#if VARIABLES_ARE_CASE_SENSITIVE
            Name = name;
#else
            Name = name.ToLowerInvariant();
#endif
        }
        public virtual void Dispose() {
        }
        public abstract PartType Type { get; } 

        public object Execute(IExecutionContext ctx, params object[] parms) {
            return GetGenericValue(ctx);
        }

        public abstract dynamic GetGenericValue(IExecutionContext ctx);

        protected virtual string GetTraceString(IExecutionContext ctx) => ToString();
    }
}