using System.IO;
using FSQL.Interfaces;
using FSQL.ProgramParts.Core;

namespace FSQL.ProgramParts.FSQLStructs {
    public class Alias : IAlias {
        public Alias(string name, string location, string filter = "*.*") {
            Name = name;
            Location = location;
            FileSpec = filter ?? "*.*";
        }
        public string Name { get; }
        public string Location { get; }

        public string FileSpec { get; }
        
        public bool IsFolder => Directory.Exists(Location);
        public bool IsFile => File.Exists(Location);
        public bool Exists => IsFolder || IsFile;

         public IFolderTable Open() => new FolderTable(this, Settings.EnableChecksums);

        public override string ToString() => $"{Name} => {Location}\\{FileSpec}";
    }
}