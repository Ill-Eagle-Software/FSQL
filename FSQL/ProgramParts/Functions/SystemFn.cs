using System;
using FSQL.Interfaces;
using FSQL.ProgramParts.Statements;

namespace FSQL.ProgramParts.Functions {
    public class SystemFn : InternalFunction
    {
        private readonly string _name;
        private readonly Func<IExecutionContext, object[], object> _impl;

        public SystemFn(string name, Func<IExecutionContext, object[], object> impl) : base(null) {
            _name = name;
            _impl = impl;
        }

        protected override object OnInvoke(IExecutionContext ctx, params object[] parms)
        {
            return _impl(ctx, parms);
        }

        public override string ToString() => $"{_name}()";
    }
}