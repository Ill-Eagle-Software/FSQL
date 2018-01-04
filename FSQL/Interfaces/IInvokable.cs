namespace FSQL.Interfaces {
    public interface IInvokable : IExecutable {
        object InvokeGeneric(IExecutionContext ctx, params object[] parms);
    }

    public interface IInvokable<out T> : IInvokable
    {
        T Invoke(IExecutionContext ctx, params object[] parms);
    }

}