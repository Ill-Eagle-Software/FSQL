using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs.DataSetFormatters
{
    public class CsvFormatter : DataSetFormatterBase<string> {
        public CsvFormatter(IExpression dataSetRetriever) : base(dataSetRetriever) {}
        public override string GetValue(IExecutionContext ctx) {
            var data = GetDataTable(ctx);
            var sb = new StringBuilder();

            var colIdx = 1;
            foreach (DataColumn col in data.Columns) {                
                sb.Append($"\"{col.ColumnName}\"");
                if (colIdx++ != data.Columns.Count) sb.Append(",");
            }
            sb.AppendLine();

            foreach (DataRow row in data.Rows)
            {
                colIdx = 0;
                foreach (DataColumn col in data.Columns)
                {                    
                    var value = row[colIdx++];
                    long test;
                    if (long.TryParse(value.ToString(), out test))
                    {
                        sb.Append(value);
                    }
                    else
                    {
                        sb.Append($"\"{value}\"");
                    }
                    if (colIdx != data.Columns.Count) sb.Append(",");                    
                }
                sb.AppendLine();
            }            
            return sb.ToString();
        }

        public override string ToString() => base.ToString() + " FORMAT AS CSV";
    }
}
