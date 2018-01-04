using System;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs.DataSetFormatters {
    public class TextFormatter : DataSetFormatterBase<string>
    {
        public TextFormatter(IExpression dataSetRetriever) : base(dataSetRetriever) {}

        public override string GetValue(IExecutionContext ctx) {
            var dt = GetDataTable(ctx);
            return FormatDataTable(dt);
        }

        public virtual string FormatDataTable(DataTable dt) { 
            var sb = new StringBuilder("|");
            
            const int colWidth = -25;
            foreach (DataColumn c in dt.Columns)
            {
                sb.Append($"{c.ColumnName,colWidth}|");
            }
            sb.AppendLine();
            sb.Append("|");
            foreach (DataColumn c in dt.Columns)
            {
                sb.Append($"{new string('=', Math.Abs(colWidth)),colWidth}+");
            }
            sb.AppendLine();
            sb.Append("|");

            foreach (DataRow r in dt.Rows)
            {
                var colIdx = 0;
                foreach (DataColumn c in dt.Columns)
                {
                    sb.Append($"{r[colIdx++],colWidth}|");
                }
                sb.AppendLine();
                sb.Append("|");
            }
            return sb.ToString().TrimEnd("|".ToCharArray());
        }

        public override string ToString() => base.ToString() + " FORMAT AS TEXT";


    }
}