using System.Collections.Generic;
using System.Linq;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.Core
{
    public class ProgramPartList<T> : List<T>, IProgramPart where T : IProgramPart
    {
        public PartType Type => this.FirstOrDefault() == null ? PartType.Constant : (this.First().Type);

        public void Dispose() {
            foreach (var x in this) x.Dispose();
        }

        public override string ToString() => $"PartsList: Contents->{typeof(T).Name}, Count->{Count}";
    }
}
