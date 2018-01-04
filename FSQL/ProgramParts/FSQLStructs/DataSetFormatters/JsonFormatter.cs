using System.Data;
using System.Text;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs.DataSetFormatters {
    public class JsonFormatter : DataSetFormatterBase<string>
    {
        public JsonFormatter(IExpression dataSetRetriever) : base(dataSetRetriever) {}
        public override string GetValue(IExecutionContext ctx) {
            var data = GetDataTable(ctx);
            var sb = new StringBuilder();
            sb.AppendLine("[");
            var rowIdx = 1;
            foreach (DataRow row in data.Rows) {
                sb.AppendLine("   {");
                var colIdx = 0;
                foreach (DataColumn col in data.Columns) {                    
                    var name = col.ColumnName;
                    var value = row[colIdx++];
                    long test;
                    if (long.TryParse(value.ToString(), out test)) {
                        sb.Append($"      \"{name}\" : {value}");
                    } else {
                        sb.Append($"      \"{name}\" : \"{value}\"");
                    }
                    sb.AppendLine(colIdx == data.Columns.Count ? "" : ",");
                }
                sb.Append("   }");
                sb.AppendLine(rowIdx++ == data.Rows.Count ? "" : ",");
            }
            sb.AppendLine("]");
            return sb.ToString();
        }

        public override string ToString() => base.ToString() + " FORMAT AS JSON";
    }
}