using System;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.Expressions {
    public class OneTermExpression : IExpression
    {
        private readonly IExpression _left;        
        private readonly string Operator;
        private Func<dynamic, dynamic> _transform;

        public static OneTermExpression Create(IExpression left, Func<dynamic, dynamic> xformer, string oper = null)
            => new OneTermExpression(left, xformer, oper);

        public OneTermExpression(IExpression left, Func<dynamic, dynamic> xformer, string oper = null)
        {
            _left = left;
            _transform = xformer;
            Operator = oper ?? "_";
        }

        public void Dispose()
        {
            _left.Dispose();
            _transform = null;
        }
        public PartType Type { get; } = PartType.Expression;

        public dynamic Execute(IExecutionContext ctx, params object[] parms)
            => GetGenericValue(ctx);

        public dynamic GetGenericValue(IExecutionContext ctx)
        {
            var left = _left.GetGenericValue(ctx);
            var results = _transform(left);
            return results;
        }

        public override string ToString() => $"{Operator} /{_left}/";
    }
}