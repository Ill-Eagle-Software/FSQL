using System.Data;
using System.Security.Cryptography.X509Certificates;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs.DataSetFormatters {
    public abstract class DataSetFormatterBase<T> : IDataSetFormatter, IExpression<T> {
        public IExpression DataSetRetriever { get; }

        protected DataSetFormatterBase(IExpression dataSetRetriever) {
            DataSetRetriever = dataSetRetriever;
        }
        public abstract T GetValue(IExecutionContext ctx);
        public PartType Type { get; } = PartType.Expression;

        public virtual void Dispose() {
            DataSetRetriever?.Dispose();
        }

        protected DataTable GetDataTable(IExecutionContext ctx) => DataSetRetriever.GetGenericValue(ctx) as DataTable;
        public object Execute(IExecutionContext ctx, params object[] parms) => GetValue(ctx);

        public object GetGenericValue(IExecutionContext ctx) => GetValue(ctx);

        public override string ToString() => DataSetRetriever.ToString();
    }
}