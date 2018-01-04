using System;
using System.Collections.Generic;
using System.Linq;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.Statements {
    public class WriteLine : InternalFunction
    {
        protected override object OnInvoke(IExecutionContext ctx, params object[] parms)
        {
            if (parms.Length == 0) {
                Console.WriteLine();
                return 0;
            }
            foreach (var obj in parms) {
                Console.Write(obj.ToString());
            }
            Console.WriteLine();
            return parms.Length;
        }

        public WriteLine(IEnumerable<IExpression> parameters) : base(parameters) {}

        public override string ToString() {            
            return ($"Writeline({Parameters.Count()} parameters);");
        }
    }
}