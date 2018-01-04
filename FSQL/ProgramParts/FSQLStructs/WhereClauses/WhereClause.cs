using System.Data;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs.WhereClauses {
    public class WhereClause : WhereClauseBase {
        public IExpression Expr { get; set; }

        public WhereClause(IExpression expr) {
            Expr = expr;
        }

        public override bool GetValue(IExecutionContext ctx) {
            return (bool) Expr.GetGenericValue(ctx);
        }

        public override bool Test(IExecutionContext ctx, DataRow dr) {
            return (bool) Expr.GetGenericValue(ctx);
        }

        public override string ToString() => Expr.ToString();
    }
}