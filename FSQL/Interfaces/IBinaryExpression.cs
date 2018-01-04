namespace FSQL.Interfaces {
    public interface IBinaryExpression : IUnaryExpression
    {
        Operator Operator { get; }
        IProgramPart Right { get; }    
    }
}