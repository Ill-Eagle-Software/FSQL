using System;

namespace FSQL.ProgramParts.DataTypes {
    public class DoubleConstant : Constant<double>
    {
        public DoubleConstant(double value) : base(value) { }

        public static implicit operator DoubleConstant(double value) => new DoubleConstant(value);
        public static implicit operator double(DoubleConstant value) => value.TypedValue;

        public static implicit operator IntConstant(DoubleConstant value) => new IntConstant(Convert.ToInt32(value.TypedValue));
        public static implicit operator DoubleConstant(IntConstant value) => new DoubleConstant(Convert.ToDouble(value.TypedValue));
    }

}