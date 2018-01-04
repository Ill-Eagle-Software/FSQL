using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs.DataSetFormatters {
    public static class DataSetFormatterFactory {
        public static IExpression Build(string format, IExpression select) {
            IExpression results; 
            switch (format) {
                case "TEXT":
                    results = new TextFormatter(select);
                    break;
                case "JSON":
                    results = new JsonFormatter(select);
                    break;
                case "CSV":
                    results = new CsvFormatter(select);
                    break;
                case "SCALAR":
                    results = new ScalarFormatter(select);
                    break;
                default:
                    results = new NullDataSetFormatter(select);
                    break;
            }
            return results;
        }
    }
}