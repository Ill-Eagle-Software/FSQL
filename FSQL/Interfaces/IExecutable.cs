namespace FSQL.Interfaces {
    public interface IExecutable {
        object Execute(IExecutionContext ctx, params object[] parms);
    }
}