using System;
using FSQL.Interfaces;
using FSQL.ProgramParts.Core;

namespace FSQL.ProgramParts.DataTypes {
    public class Constant<T> : ProgramPart, IUnaryExpression {

        public override PartType Type { get; } = PartType.Expression;
        public IProgramPart Left => this;

        public Constant(T value)
        {
            TypedValue = value;
        }

        public T TypedValue { get; }

        public object GetGenericValue(IExecutionContext ctx) => TypedValue;

        public override void Dispose()
        {
            (TypedValue as IDisposable).Dispose();
        }

        public static implicit operator Constant<T>(T value) => new Constant<T>(value);
        public static implicit operator T(Constant<T> value) => value.TypedValue;

        object IExecutable.Execute(IExecutionContext ctx, params object[] parms) => GetGenericValue(ctx);

        public override string ToString() => TypedValue.ToString();

        public T Value => TypedValue;
    }








}