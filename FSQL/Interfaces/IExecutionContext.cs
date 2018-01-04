using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using FSQL.ProgramParts;
using FSQL.ProgramParts.FSQLStructs;

namespace FSQL.Interfaces {
    public interface IExecutionContext
    {
        string Name { get; }

        ExecutionState State { get; }

        IExecutionContext Parent { get; }
        bool IsRoot { get; }
        object this[string name] { get; set; }
        bool HasVariable(string name);

        Stack<dynamic> Stack { get; }
        dynamic Variables { get; }
        int Level { get; }
        string ContextPath { get; }

        bool InSimulationMode { get; set; }

        object Export(string name, object value);
        object Export(string name);

        Priority TraceLevel { get; set; }
        bool TracingEnabled { get; set; }
        void Trace(string message, Priority lvl = Priority.Normal);
        IEnumerable<string> TraceMessages { get; }  
              
        IEnumerable<Alias> Aliases { get; }
        IExecutionContext AddAlias(string name, string folder, string filter = "*.*", bool replace = false);
        IExecutionContext AddAlias(Alias alias, bool replace = false);
        bool RemoveAlias(string name);
        Alias GetAlias(string name);

        IExecutionContext Enter(string name, IDictionary<string, object> variables = null);

        //IExecutionContext Resume(object returnValue, ExecutionState state = ExecutionState.Ok);
        IExecutionContext Resume(IExecutionContext childContext);
        IExecutionContext Exit(object returnValue = null);

        void SetReturnValue(dynamic value);
        dynamic ReturnValue { get; }

        #region Debugging Tools

        bool ThrowOnException { get; set; }

        /// <summary>
        /// Logs the exception into the execution context. If the return value is true, then the caller should also throw the exception.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="traceMsg"></param>
        /// <returns>The current <see cref="ThrowOnException"/> value. If the return value is true, then the caller should throw the exception.</returns>
        bool Throw(Exception ex, string traceMsg = null);

        Exception Exception { get; }

        string Dump(Priority level = Priority.Normal);

        #endregion
    }

    public enum Priority {                
        Terse,
        Normal,
        Verbose
    }
}