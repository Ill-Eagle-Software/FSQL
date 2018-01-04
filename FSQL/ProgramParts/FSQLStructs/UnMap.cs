using FSQL.Interfaces;
using FSQL.ProgramParts.Core;

namespace FSQL.ProgramParts.FSQLStructs {
    public class UnMap : StatementBase {
        public string Aliasname { get; set; }

        public UnMap(string aliasname) {
            Aliasname = aliasname;
        }
        protected override object OnExecute(IExecutionContext ctx, params object[] parms) => ctx.RemoveAlias(Aliasname);        

        public override void Dispose() {            
        }

        public override string ToString() =>
            $"UNMAP {Aliasname};";
    }
}