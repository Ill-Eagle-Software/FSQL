using System.Data;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs.Sources {


    public class JoinSourceAggregator : Source
    {
        private readonly AliasRef _left;
        private readonly AliasRef _right;
        private readonly IExpression _join;
        protected string NewAlias { get; set; }

        public JoinSourceAggregator(AliasRef left, AliasRef right, IExpression join, string alias) {
            _left = left;
            _right = right;            
            _join = @join;
            NewAlias = alias;
        }


        public override AliasRef Alias => _left;
        public virtual AliasRef JoinedAlias => _right;

        public override ITable GetValue(IExecutionContext ctx) {
            var leftA = _left.GetValue(ctx);
            var rightA = _right.GetValue(ctx);
            ctx.Trace($"Joining {leftA.Name} with {rightA.Name}.", Priority.Verbose);

            var left = RegisterAlias(ctx, leftA);
            var right = RegisterAlias(ctx, rightA);

            NewAlias = NewAlias ?? $"{leftA.Name}#{rightA.Name}";
                    
            var results = Join(ctx, left, right);

            return new RowSet(NewAlias, results);
        }

        public override void Dispose() {
            _left.Dispose();
            _right.Dispose();
            _join.Dispose();
        }

        protected virtual DataTable Join(IExecutionContext ctx, IActiveRecord left, IActiveRecord right)
        {
            var results = BuildMergeResultsSet(left, right);
            var condition = _join;
            var leftIdx = left.Index;
            var rightIdx = right.Index;

            for (left.MoveFirst(); !left.IsAtEnd; left.MoveNext())
            {
                for (right.MoveFirst(); !right.IsAtEnd; right.MoveNext())
                {
                    if (!left.IsAtEnd && !right.IsAtEnd && (bool) condition.GetGenericValue(ctx)) {
                        var dr = AddMergeResultRow(results, left.CurrentRow, right.CurrentRow);
                    }
                }
            }

            left.GoTo(leftIdx);
            right.GoTo(rightIdx);
            return results;
        }

        protected virtual DataTable BuildMergeResultsSet(IActiveRecord left, IActiveRecord right) {
            var colL = left.GetData().Columns;
            var colR = right.GetData().Columns;
            var results = new DataTable();
            foreach (DataColumn c in colL) results.Columns.Add(new DataColumn($"{left.TableName}.{c.ColumnName}"));
            foreach (DataColumn c in colR) results.Columns.Add(new DataColumn($"{right.TableName}.{c.ColumnName}"));
            return results;
        }

        protected virtual DataRow AddMergeResultRow(DataTable tbl, DataRow left, DataRow right)
        {
            var colIdx = 0;
            var srcIdx = 0;
            var row = tbl.NewRow();
            while (srcIdx < left.ItemArray.Length)
            {
                row[colIdx++] = left[srcIdx++];
            }
            srcIdx = 0;
            while (srcIdx < right.ItemArray.Length)
            {
                row[colIdx++] = right[srcIdx++];
            }
            tbl.Rows.Add(row);
            return row;
        }

        public override string ToString() {
            return $"{_left} JOIN {_right} ON {_join}";
        }
    }


}