using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs {
    public class AliasRef : IExpression<Alias> {

        public static int AliasesNamesAutoAssigned = 0;
        public AliasRef(string aliasName, string folder) {
            if (string.IsNullOrWhiteSpace(aliasName)) {
                AliasName = $"FOLDER_{AliasesNamesAutoAssigned++}";
            } else {
                AliasName = aliasName;
            }
            if (string.IsNullOrWhiteSpace(folder)) {
                folder = null;
            }
            else
            {
                Folder = folder;
            }           
        }

        public PartType Type { get; } = PartType.Expression;

        public string AliasName { get; }
        public string Folder { get; }

        public Alias GetValue(IExecutionContext ctx) {

            // Do we have this alias already defined in the context?
            var results = ctx.GetAlias(AliasName);
            if (results != null) return results;

            var alias= new Alias(AliasName, Folder);
            if (!string.IsNullOrWhiteSpace(Folder))
                ctx.AddAlias(alias);
            return alias;
        }

        public void Dispose() {}

        public override string ToString() =>
            Folder == null ?
                $"{AliasName}" :
                $"\"{Folder}\" {AliasName}";

        //$"{AliasName} ({(Folder ?? "-specified elsewhere-")})";

    }
}