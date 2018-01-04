using System;

namespace FSQL.Interfaces {
    public interface IScriptEngine {

        IScript Build(string script, Action<IExecutionContext> contextInitializer = null);

        dynamic Execute(string script, params object[] parms);

        dynamic Run(string filename, params object[] parms);

        dynamic Run(string filename, Action<IExecutionContext> contextInitializer, params object[] parms);

        IStatement Parse(string source);

        IExecutionContext Debug(string script, params object[] parms);
        IExecutionContext Debug(string script, Action<IExecutionContext> contextInitializer, params object[] parms);
    }
}