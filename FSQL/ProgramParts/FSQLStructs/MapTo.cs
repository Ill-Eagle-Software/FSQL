using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSQL.Interfaces;
using FSQL.ProgramParts.Core;

namespace FSQL.ProgramParts.FSQLStructs
{
    public class MapTo : StatementBase
    {
        public string AliasName { get; set; }
        public IExpression Folder { get; set; }
        public IExpression Filter { get; set; }

        public MapTo(string aliasname, IExpression folder, IExpression filter) {
            AliasName = aliasname;
            Folder = folder;
            Filter = filter;
        }

        protected override object OnExecute(IExecutionContext ctx, params object[] parms) {            
            var folder = Folder.GetGenericValue(ctx).ToString().Unescape();
            var filter = Filter.GetGenericValue(ctx).ToString().Unescape();
            var alias = new Alias(AliasName, folder, filter);
            ctx.AddAlias(alias);
            return alias;
        }

        public override void Dispose() {
            Folder.Dispose();
            Filter.Dispose();
        }

        public override string ToString() => $"MAP {AliasName} TO {Folder} WITH FILTER {Filter};";
        
    }
}
