using System.Data;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs.DataSetFormatters {
    public class NullDataSetFormatter : DataSetFormatterBase<DataTable> {
        public NullDataSetFormatter(IExpression dataSetRetriever) : base(dataSetRetriever) {}
        public override DataTable GetValue(IExecutionContext ctx) => GetDataTable(ctx);
        public override void Dispose() {}
    }
}