using FSQL.Interfaces;

namespace FSQL.ProgramParts.Variables {
    public class VariableGet : VariableBase {
        public VariableGet(string name) : base(name) {}
        public override PartType Type { get; } = PartType.Expression;

        public override dynamic GetGenericValue(IExecutionContext ctx) {
            return ctx[Name];
        }
        public override string ToString()
        {
            return $"@{Name}";
        }
    }
}