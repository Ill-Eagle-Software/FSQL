using System.Collections.Generic;
using System.Data;
using FSQL.Interfaces;
using FSQL.ProgramParts.FSQLStructs.SelectObjects;

namespace FSQL.ProgramParts.FSQLStructs.WhereClauses {
    public class WhereInSubqueryClause : WhereClauseBase {
        private readonly ScopedAttribute _attribute;
        private readonly string _attrName;
        private readonly SelectFrom _subSelect;
        private readonly bool _negate;

        public WhereInSubqueryClause(ScopedAttribute attribute, SelectFrom subSelect, bool negate = false) {
            _attribute = attribute;
            _attrName = attribute.ColumnName;
            _subSelect = subSelect;
            _negate = negate;
        }

        public override bool GetValue(IExecutionContext ctx) {
            return false;
        }

        public override void Initialize(IExecutionContext ctx) {
            base.Initialize(ctx);
            var subQ = _subSelect.GetGenericValue(ctx) as DataTable;
            data = new List<object>();
            foreach (DataRow dr in subQ.Rows) {
                data.Add(dr[0]);
            }
        }

        private List<object> data;

        public override bool Test(IExecutionContext ctx, DataRow dr) {
            var value = dr[_attrName];
            var contained = data.Contains(value);
            return _negate ? !contained : contained;
        }

        public override string ToString() {
            return $"{_attribute.CanonicalName} {(_negate ? "NOT" : "")} IN ({_subSelect})";
        }
    }
}