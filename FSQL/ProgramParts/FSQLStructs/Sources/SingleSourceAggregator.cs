using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs.Sources
{
    public class SingleSourceAggregator : Source
    {
        public override AliasRef Alias { get; }
        public SingleSourceAggregator(AliasRef alias) {
            Alias = alias;
        }

        public override ITable GetValue(IExecutionContext ctx) {
            var alias = Alias.GetValue(ctx);
            return RegisterAlias(ctx, alias);
        }

        public override void Dispose() {
            Alias.Dispose();
        }
       
        public override string ToString() => Alias.ToString();
    }
}
