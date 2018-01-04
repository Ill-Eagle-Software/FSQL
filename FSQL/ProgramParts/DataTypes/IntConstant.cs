using System;

namespace FSQL.ProgramParts.DataTypes {
    public class IntConstant : Constant<int> {
        public IntConstant(int value) : base(value) {}

        public static implicit operator IntConstant(int value) => new IntConstant(value);
        public static implicit operator int(IntConstant value) => value.TypedValue;

        public static implicit operator IntConstant(DoubleConstant value) => new IntConstant(Convert.ToInt32(value.TypedValue));
        public static implicit operator DoubleConstant(IntConstant value) => new DoubleConstant(Convert.ToDouble(value.TypedValue));        
    }

}