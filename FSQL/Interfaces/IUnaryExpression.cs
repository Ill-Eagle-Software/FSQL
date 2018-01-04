namespace FSQL.Interfaces {
    public interface IUnaryExpression : IProgramPart, IExpression {
        IProgramPart Left { get; }
    }
}