using System.Data;
using System.Windows.Forms;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs.Destinations {
    public class ClipboardDestination : DestinationBase
    {
        public ClipboardDestination(IExpression dataRetriever) : base(dataRetriever) { }

        protected override void OnDataReceived(IExecutionContext ctx, object data)
        {
            if (data is string) {
                Clipboard.SetText(data + string.Empty);
                //Console.WriteLine(data);
            }
            if (data is DataTable)
            {
                var dt = data as DataTable;
                Clipboard.SetDataObject(dt, true);
            }
        }
    }
}