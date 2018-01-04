using System.Collections.Generic;
using FSQL.Interfaces;
using FSQL.ProgramParts.Core;

namespace FSQL.ProgramParts.Functions {
    public class ParamBlock : ProgramPart, IExpression<IEnumerable<string>> {
        private readonly IEnumerable<string> _parameterNames;
        public override PartType Type { get; } = PartType.ParameterList;

        public ParamBlock(params string[] parameterNames) {
            _parameterNames = parameterNames;
        }

        public IEnumerable<string> GetValue(IExecutionContext ctx) => _parameterNames;
        public object GetGenericValue(IExecutionContext ctx) => _parameterNames;
        public override void Dispose() { }
    }
}