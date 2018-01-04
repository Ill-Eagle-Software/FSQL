using System;

namespace FSQL.ProgramParts.DataTypes {
    public class DateTimeConstant : Constant<DateTime>
    {
        public DateTimeConstant(DateTime value) : base(value) { }

        public static implicit operator DateTimeConstant(DateTime value) => new DateTimeConstant(value);
        public static implicit operator DateTime(DateTimeConstant value) => value.TypedValue;
    }
}