using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs.DataSetFormatters {
    public class ScalarFormatter : DataSetFormatterBase<object>
    {
        public ScalarFormatter(IExpression dataSetRetriever) : base(dataSetRetriever) { }
        public override object GetValue(IExecutionContext ctx) {
            var data = GetDataTable(ctx);
            if (data.Rows.Count == 0 || data.Columns.Count == 0) return null;
            return data.Rows[0][0];
        }

        public override string ToString() => base.ToString() + " FORMAT AS SCALAR";


    }
}