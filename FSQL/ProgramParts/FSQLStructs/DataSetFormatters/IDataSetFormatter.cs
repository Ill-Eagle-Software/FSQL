using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs.DataSetFormatters {
    public interface IDataSetFormatter : IExpression {
        IExpression DataSetRetriever { get; }
    }
}