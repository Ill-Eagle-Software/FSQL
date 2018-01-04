namespace FSQL.Interfaces {
    public interface IExpression : IProgramPart, IExecutable
    {
        object GetGenericValue(IExecutionContext ctx);
    }

    public interface IExpression<out T> : IProgramPart
    {
        T GetValue(IExecutionContext ctx);
    }
}