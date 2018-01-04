using System;
using System.IO;
using System.Security.Cryptography;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs {
    //public class FileInformation : IFileInformation {
    //    private readonly string _fullPath;
    //    private long _size = -1;
    //    private DateTime _created = DateTime.MaxValue;
    //    private DateTime _modified = DateTime.MaxValue;

    //    public FileInformation(string path) {
    //        _fullPath = System.IO.Path.GetFullPath(path);
    //    }

    //    public string FullName => _fullPath;
    //    public string Path => System.IO.Path.GetDirectoryName(_fullPath);
    //    public string Name => System.IO.Path.GetFileName(_fullPath);
    //    public string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(_fullPath);
    //    public string Extension => System.IO.Path.GetExtension(_fullPath);
    //    public long Size => _size > -1 ? _size : (_size = (new FileInfo(_fullPath)).Length) ;
    //    public DateTime CreatedUtc => _created != DateTime.MaxValue ? _created : (_created = (new FileInfo(_fullPath)).CreationTimeUtc);
    //    public DateTime ModifiedUtc => _modified != DateTime.MaxValue ? _modified : (_modified = (new FileInfo(_fullPath)).LastWriteTimeUtc);
    //}
}