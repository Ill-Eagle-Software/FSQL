using System;

namespace FSQL.Interfaces {
    public interface IProgramPart: IDisposable {
        PartType Type { get; }
    }
}