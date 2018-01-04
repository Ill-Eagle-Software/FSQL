using System;
using System.Data;
using FSQL.Interfaces;
using FSQL.ProgramParts.FSQLStructs.DataSetFormatters;

namespace FSQL.ProgramParts.FSQLStructs.Destinations {
    public class ScreenDestination : DestinationBase
    {
        public ScreenDestination(IExpression dataRetriever) : base(dataRetriever) { }

        protected override void OnDataReceived(IExecutionContext ctx, object data) {
            if (data is string) Console.WriteLine(data);
            if (data is DataTable) {                
                var dt = data as DataTable;
                using (var fmt = new TextFormatter(null))
                    Console.WriteLine(fmt.FormatDataTable(dt));
            }
        }
    }
}