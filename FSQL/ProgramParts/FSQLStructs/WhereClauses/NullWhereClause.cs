using System.Data;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs.WhereClauses {
    public sealed class NullWhereClause : WhereClauseBase {
        public override bool GetValue(IExecutionContext ctx) => true;
        public override bool Test(IExecutionContext ctx, DataRow dr) => true;
    }
}