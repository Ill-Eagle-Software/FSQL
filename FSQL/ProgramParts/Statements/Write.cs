using System;
using System.Collections.Generic;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.Statements {
    public class Write : InternalFunction
    {
        protected override object OnInvoke(IExecutionContext ctx, params object[] parms)
        {
            foreach (var obj in parms)
            {
                Console.Write(obj.ToString());
            }
            return parms.Length;
        }

        public Write(IEnumerable<IExpression> parameters) : base(parameters) {}
    }
}