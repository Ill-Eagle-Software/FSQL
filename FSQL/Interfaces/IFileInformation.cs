using System;

namespace FSQL.Interfaces {
    public interface IFileInformation {
        string FullName { get; }
        string Path { get; }
        string Name { get; }
        string NameWithoutExtension { get; }
        string Extension { get; }
        long Size { get; }
        DateTime CreatedUtc { get; } 
        DateTime ModifiedUtc { get; }
    }
}