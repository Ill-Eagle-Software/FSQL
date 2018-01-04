using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FSQL.Interfaces;
using FSQL.ProgramParts.Core;
using FSQL.ProgramParts.FSQLStructs.Sources;
using FSQL.ProgramParts.FSQLStructs.WhereClauses;

namespace FSQL.ProgramParts.FSQLStructs.SelectObjects {
    public class SelectFrom : StatementBase, IExpression {
                        
        public SelectFrom(List<ScopedAttribute> requestedAttributes, Source dataSource, WhereClauseBase whereClause) {
            RequestedAttributes = requestedAttributes;
            Source = dataSource;
            WhereClause = whereClause;
        }

        private List<ScopedAttribute> RequestedAttributes { get; }

        private Source Source { get; }
        private WhereClauseBase WhereClause { get; set; }

        protected override object OnExecute(IExecutionContext ctx, params object[] parms) {
            WhereClause.Initialize(ctx);
            var results = GetRows(ctx);
            return results;
        }

        public override void Dispose() { }
        public object GetGenericValue(IExecutionContext ctx) => InvokeGeneric(ctx);

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("SELECT ");
            sb.Append(string.Join(",", RequestedAttributes.Select(r => r.ToString())));
            sb.Append(" FROM ");
            sb.Append(string.Join(",", Source.ToString()));
            if (!(WhereClause is NullWhereClause)) {
                sb.Append(" WHERE ");
                sb.Append(WhereClause);
            }
            //sb.AppendLine(";");
            return sb.ToString();
        }

        protected DataTable GetRows(IExecutionContext ctx) {
            DataTable results = null;
            IExecutionContext sqlCtx = null; 
            try
            { // Enter Query Context. This allows the query to set up temporary local variables that are removed when the query goes out of scope.                            
                sqlCtx = ctx.Enter("__SQL");
                var data = Source.GetValue(sqlCtx);
                var dTable = (WhereClause is NullWhereClause) 
                    ? data.GetData() 
                    : data.Filter((DataRow dr) => WhereClause.Test(sqlCtx, dr));
                results = dTable.ReduceColumnsToAliases(RequestedAttributes);
            } finally {
                sqlCtx?.Exit(results);
            }
            return results;
        }

        protected virtual DataTable BuildResultsTable() {
            var results = new DataTable();
            foreach (var col in RequestedAttributes) {
                results.Columns.Add(col.ColumnName);
            }
            return results;
        }

        protected virtual bool IsMatch(List<ITable> tables) => true;

    }
}