namespace FSQL.ProgramParts.DataTypes {
    public class BoolConstant : Constant<bool>
    {
        private BoolConstant(bool value) : base(value) { }

        public static implicit operator BoolConstant(bool value) => new BoolConstant(value);
        public static implicit operator bool(BoolConstant value) => value.TypedValue;

        private static readonly BoolConstant _true = new BoolConstant(true);
        private static readonly BoolConstant _false = new BoolConstant(false);

        public static BoolConstant True => _true;
        public static BoolConstant False => _false;
    }
}