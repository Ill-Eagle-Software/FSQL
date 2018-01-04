using System.Data;
using System.IO;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs.Destinations {
    public class FileDestination : DestinationBase
    {
        public IExpression Filename { get; set; }
        public FileDestination(IExpression filename, IExpression dataRetriever) : base(dataRetriever) {
            Filename = filename;
        }

        protected override void OnDataReceived(IExecutionContext ctx, object data) {
            var filename = Filename.GetGenericValue(ctx).ToString().Unescape();
            if (data is string)
            {
                File.WriteAllText(filename, data + string.Empty);
                //Console.WriteLine(data);
            }
            if (data is DataTable)
            {
                var dt = data as DataTable;
                dt.WriteXml(filename, XmlWriteMode.WriteSchema);
            }
        }
    }
}