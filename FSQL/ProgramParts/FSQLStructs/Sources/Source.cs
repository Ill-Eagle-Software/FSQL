using FSQL.Interfaces;
using FSQL.ProgramParts.Core;

namespace FSQL.ProgramParts.FSQLStructs.Sources {
    public abstract class Source : IExpression<ITable> {
        public PartType Type { get; } = PartType.Expression;

        public abstract AliasRef Alias { get; }

        public abstract ITable GetValue(IExecutionContext ctx);         

        public virtual void Dispose() {}

        protected virtual ITable RegisterAlias(IExecutionContext ctx, Alias alias)
        {
            var table = alias.Open();
            ctx[$"{Constants.TablePrefix}{alias.Name}"] = table;
            return table;
        }

    }
}