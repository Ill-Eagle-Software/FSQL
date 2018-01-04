using FSQL.Interfaces;
using FSQL.ProgramParts.DataTypes;

namespace FSQL.ProgramParts.FSQLStructs.Destinations {
    public static class DestinationFactory {

        public static IExpression Build(IExpression formatExpr, IExpression select) {

            var format = formatExpr is StringConstant ? formatExpr.ToString() : "FILE";

            IExpression results;
            switch (format)
            {
                case "NONE":
                    results = select;
                    break;
                case "SCREEN":
                    results = new ScreenDestination(select);
                    break;
                case "CLIPBOARD":
                    results = new ClipboardDestination(select);
                    break;
                case "FILE":                    
                default: // File
                    results = new FileDestination(formatExpr, select);
                    break;
            } //TODO : Fix these!
            return results;
        }
    }
}