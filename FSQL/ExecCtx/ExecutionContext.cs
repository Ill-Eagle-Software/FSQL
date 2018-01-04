using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSQL.Exceptions;
using FSQL.Interfaces;
using FSQL.ProgramParts;
using FSQL.ProgramParts.FSQLStructs;

namespace FSQL.ExecCtx {
    public class ExecutionContext : IExecutionContext, IDisposable
    {
        public static IExecutionContext Create(IDictionary<string, object> variables = null) {
            return new ExecutionContext("root", null, variables);
        }

        protected ExecutionContext(string name, IExecutionContext parent, IDictionary<string, object> variables = null) {
            Name = name;
            Variables = new VariablesWrapper(k => this[k], (k,v) => this[k] = v);
            VariableStorage = new Dictionary<string, object>();
            MyAliases = new List<Alias>();
            this["__method"] = name;
            Parent = parent;
            Stack = new Stack<dynamic>();
            if (parent == null) TraceMessages = new List<string>();
            if (variables != null) {
                foreach (var k in variables.Keys) {
                    VariableStorage[k] = variables[k];
                }
            }
        }

        public ExecutionState State { get; protected internal set; } = ExecutionState.Ok;
        public IExecutionContext Parent { get; private set; }
        public bool IsRoot => Parent == null;


        public string Name { get; }
        public int Level => IsRoot ? 0 : Parent.Level + 1;
        public string ContextPath => IsRoot ? $"//{Name}" : $"{Parent.ContextPath}/{Name}";
        
        #region Context Management
        public IExecutionContext Enter(string name, IDictionary<string, object> variables = null)
        {
            var child = new ExecutionContext(name, this, variables);
            child.TraceLevel = TraceLevel;
            child.TracingEnabled = TracingEnabled;
            child.InSimulationMode = InSimulationMode;
            return child;
        }

        public bool InSimulationMode { get; set; }

        //public IExecutionContext Resume(object returnValue, ExecutionState state = ExecutionState.Ok) {
        public IExecutionContext Resume(IExecutionContext childContext) { 
            if (!childContext.IsRoot) {
                TracingEnabled = childContext.TracingEnabled;
                TraceLevel = childContext.TraceLevel;
                State = childContext.State;
                Exception = childContext.Exception;
            }
            return this;
        }

        public IExecutionContext Exit(object returnValue = null) {
            if (IsRoot) {
                ReturnValue = returnValue;
                return this;
            } else {
                //Parent.Resume(returnValue);
                Parent.Resume(this);
            }
            var p = Parent;
            Dispose();
            return p;
        }
        
        private static bool _throwOnException = true;
        public bool ThrowOnException { get { return _throwOnException; } set { _throwOnException = value; } }

        public bool Throw(Exception ex, string traceMsg = null) {
            Exception = ex;
            State = ExecutionState.Faulted;
            if (traceMsg != null) WriteTraceMessage(traceMsg);
            return ThrowOnException;
        }
        public Exception Exception { get; protected set; }
        public void SetReturnValue(dynamic value) {
            ReturnValue = value;
        }

        public dynamic ReturnValue { get; private set; }

        #endregion

        #region Stack 

        public Stack<dynamic> Stack { get; }
        #endregion

        #region Variables

        public object this[string name]
        {
            get { return HasVariable(name) ? GetVariable(name) : false; }
            set { VariableStorage[name] = value; }
        }
       
        public bool HasVariable(string name) => VariableStorage.ContainsKey(name) || (!IsRoot && Parent.HasVariable(name));

        public dynamic Variables { get; }

        public object Export(string name, object value) {
            if (IsRoot) {
                this[name] = value;
            } else {
                Parent[name] = value;
            }
            return value;
        }

        public object Export(string name) {
            if (IsRoot) return this[name];
            if (VariableStorage.ContainsKey(name)) {
                var value = this[name];
                Export(name, value);
                return value;
            }
            return null;
        }

        protected IDictionary<string, object> VariableStorage { get; }
        private object GetVariable(string name) => VariableStorage.ContainsKey(name) ? VariableStorage[name] : Parent?[name];

        #endregion

        #region Tracing

        public Priority TraceLevel { get; set; } = Priority.Normal;
        public bool TracingEnabled { get; set; }

        public void Trace(string message, Priority lvl = Priority.Normal) {
            if (!TracingEnabled || TraceLevel < lvl || message == null) return;
            WriteTraceMessage(message, lvl);
        }

        private void WriteTraceMessage(string message, Priority lvl = Priority.Normal, int indentLevel = 0)
        {
            if (Parent != null)
                (Parent as ExecutionContext).WriteTraceMessage(message, lvl, indentLevel+1);
            else
            {
                var indicator = lvl == Priority.Terse ? "_" : (
                    lvl == Priority.Normal ? " " : "!");
                (TraceMessages as List<string>).Add($"{indicator}:{DateTime.Now:dd-MMM-yy hh:mm:ss}: {new string(' ', indentLevel * 2)}{message}");
            }
        }


        public IEnumerable<string> TraceMessages { get; }

        public override string ToString() {
            var msgs = string.Join(Environment.NewLine, TraceMessages);
            return msgs;
        }

        #endregion

        #region Aliases
        protected List<Alias> MyAliases { get; }
        public IEnumerable<Alias> Aliases => Parent?.Aliases ?? MyAliases;
        public IExecutionContext AddAlias(string name, string folder, string filter = "*.*", bool replace = false) => 
            AddAlias(new Alias(name, folder, filter), replace);
        public IExecutionContext AddAlias(Alias alias, bool replace=false) {

            //ALL Alias Management is to be done at the root.
            if (Parent != null) return Parent.AddAlias(alias, replace);

            if (replace) MyAliases.RemoveAll(a => a.Name == alias.Name);
            if (MyAliases.Exists(a => a.Name == alias.Name)) throw new RedefinedAliasException($"Alias [{alias.Name}] exists, and replace parameter is false.");
            MyAliases.Add(alias);
            return this;
        }

        public bool RemoveAlias(string aliasName) {
            if (IsRoot) {
                var alias = GetAlias(aliasName);
                if (alias == null) return false;
                MyAliases.Remove(alias);
                return true;
            } else {
                return Parent.RemoveAlias(aliasName);
            }
        }
        public Alias GetAlias(string name)
        {
            if (Parent != null) return Parent.GetAlias(name);

            var alias = MyAliases.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.InvariantCultureIgnoreCase));
            return alias;
            //if (alias == null) throw new UndefinedAliasException($"Alias '{name}' is undefined.");
            //return alias;
        }
        #endregion

        public void Dispose() {
            Parent = null;
            VariableStorage.Clear();
        }

        public string Dump(Priority level = Priority.Normal)
        {
            var sb = new StringBuilder();
            sb.AppendLine(new string('*', 80));
            sb.AppendLine("* EXECUTION CONTEXT -- CORE DUMP");
            sb.AppendLine($"* NAME : {Name}");
            sb.AppendLine(new string('*', 80));
            sb.AppendLine();

            //Terse
            sb.AppendLine($"     Context Path: {ContextPath}");
            sb.AppendLine($"    Nesting Level: {Level}");

            if (level == Priority.Terse)
            {
                sb.AppendLine($"  Local Variables: {VariableStorage.Count}");
                sb.AppendLine($"  Defined Aliases: {MyAliases.Count}");
                sb.AppendLine($"      Stack Depth: {Stack.Count}");
            }
            if (level >= Priority.Normal)
            {
                sb.AppendLine($"  Local Variables: {VariableStorage.Count}");
                foreach (var k in VariableStorage.Keys) {
                    sb.AppendLine($"                   {k,-25} : {VariableStorage[k]}");
                }
                sb.AppendLine($"  Defined Aliases: {MyAliases.Count}");
                foreach (var a in Aliases)
                {
                    sb.AppendLine($"                   {a.Name,-25} : {a.Location}");
                }                
                sb.AppendLine($"      Stack Depth: {Stack.Count}");
                var stk = new dynamic[Stack.Count];
                Stack.CopyTo(stk,0);
                foreach (var s in stk)
                {
                    sb.AppendLine($"                   {s.ToString()}");
                }
            }
            if (State == ExecutionState.Faulted)
            {
                sb.AppendLine($" Script Exception: {Exception.GetType().Name}");
                var ex = Exception;
                var lvl = 1;
                while (ex != null) {
                    sb.AppendLine($"                   {new string(' ', lvl * 2)}{ex.Message}");
                    ex = ex.InnerException;
                    lvl++;
                }

            }
            if (level == Priority.Verbose)
            {
                if (TraceMessages != null) {
                    sb.AppendLine($"        Trace Log: {VariableStorage.Count}");
                    foreach (var s in TraceMessages) {
                        sb.AppendLine($"                   {s.Replace("{", "{{").Replace("}", "}}")}");
                    }
                }
            }
            sb.AppendLine(new string('*', 80));
            return sb.ToString();
        }

    }
}