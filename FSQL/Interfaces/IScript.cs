namespace FSQL.Interfaces {
    public interface IScript
    {
        dynamic InvokeGeneric(IExecutionContext ctx, params object[] parms);

        dynamic InvokeGeneric(params object[] parms);
        IExecutionContext Debug(IExecutionContext ctx, params object[] parms);
        IExecutionContext Debug(params object[] parms);
    }
}