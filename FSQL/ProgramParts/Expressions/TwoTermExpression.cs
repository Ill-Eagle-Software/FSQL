using System;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.Expressions {
    public class TwoTermExpression : IExpression
    {
        private readonly IExpression _left;
        private readonly IExpression _right;
        private readonly string Operator;
        private Func<dynamic, dynamic, dynamic> _transform;

        public static TwoTermExpression Create(IExpression left, IExpression right, Func<dynamic, dynamic, dynamic> xformer, string oper = null)
            => new TwoTermExpression(left, right, xformer, oper);

        public TwoTermExpression(IExpression left, IExpression right, Func<dynamic, dynamic, dynamic> xformer, string oper=null)
        {
            _left = left;
            _right = right;
            _transform = xformer;
            Operator = oper ?? "_";
        }

        public void Dispose()
        {
            _left.Dispose();
            _right.Dispose();
            _transform = null;
        }
        public PartType Type { get; } = PartType.Expression;

        public dynamic Execute(IExecutionContext ctx, params object[] parms)
            => GetGenericValue(ctx);

        public dynamic GetGenericValue(IExecutionContext ctx)
        {
            var left = _left.GetGenericValue(ctx);
            var right = _right.GetGenericValue(ctx);
            var results = _transform(left, right);
            return results;
        }

        public override string ToString() => $"{_left} {Operator} {_right}";
    }
    public class TwoTermExpression<T> : IExpression<T>
    {
        private readonly IExpression<T> _left;
        private readonly IExpression<T> _right;
        private Func<T, T, T> _transform;

        public static TwoTermExpression<TI> Create<TI>(IExpression<TI> left, IExpression<TI> right, Func<TI, TI, TI> xformer)
            => new TwoTermExpression<TI>(left, right, xformer);

        public TwoTermExpression(IExpression<T> left, IExpression<T> right, Func<T, T, T> xformer)
        {
            _left = left;
            _right = right;
            _transform = xformer;
        }
        public T GetValue(IExecutionContext ctx)
        {
            var left = _left.GetValue(ctx);
            var right = _right.GetValue(ctx);
            var results = _transform(left, right);
            return results;
        }

        public void Dispose()
        {
            _left.Dispose();
            _right.Dispose();
            _transform = null;
        }
        public PartType Type { get; } = PartType.Expression;

        public override string ToString() => $"LEFT=/{_left}/, RIGHT=/{_right}/";
        
    }
}