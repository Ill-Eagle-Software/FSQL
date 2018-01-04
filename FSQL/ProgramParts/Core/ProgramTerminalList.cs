using System.Collections.Generic;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.Core {
    public class ProgramTerminalList<T> : List<T>, IProgramPart {
        public PartType Type => PartType.Terminal;

        public virtual void Dispose() {}

        public override string ToString() => $"PartsList: Contents->{typeof(T).Name}, Count->{Count}";
    }
}