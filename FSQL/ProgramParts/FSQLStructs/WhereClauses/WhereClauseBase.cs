using System.Data;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs.WhereClauses {
    public abstract class WhereClauseBase :IExpression<bool> {

        public PartType Type { get; } = PartType.Expression;

        public abstract bool GetValue(IExecutionContext ctx);

        public virtual void Initialize(IExecutionContext ctx) {            
        }

        public abstract bool Test(IExecutionContext ctx, DataRow dr);
         
        public virtual void Dispose() {}
        
    }
}