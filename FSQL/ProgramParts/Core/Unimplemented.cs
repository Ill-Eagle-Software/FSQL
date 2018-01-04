using System;
using FSQL.Interfaces;
using FSQL.ProgramParts.Functions;
using FSQL.ProgramParts.Statements;

namespace FSQL.ProgramParts.Core {
    public class Unimplemented : InternalFunction
    {
        private readonly string _functionname;

        public Unimplemented(string functionname) : base(null) {
            _functionname = functionname;
        }
        protected override object OnInvoke(IExecutionContext ctx, params object[] parms)
        {
            throw new NotImplementedException($"Function '{_functionname}' has not been implemented.");
        }

        public override void Dispose() { }
    }
}