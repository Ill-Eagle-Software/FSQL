using System;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.Core {
    public abstract class ProgramPart : IProgramPart, IDisposable
    {
        public abstract PartType Type { get; }
        public abstract void Dispose();
    }
}