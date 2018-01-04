namespace FSQL.ProgramParts.DataTypes {
    public class StringConstant : Constant<string>
    {
        public StringConstant(string value) : base(value) { }

        public static implicit operator StringConstant(string value) => new StringConstant(value);
        public static implicit operator string(StringConstant value) => value.TypedValue;
    }

}