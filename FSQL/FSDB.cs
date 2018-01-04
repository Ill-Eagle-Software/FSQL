using FSQL.Interfaces;
using FSQL.ProgramParts;
using FSQL.ProgramParts.Core;
using FSQL.ProgramParts.FSQLStructs;

namespace FSQL {
    public class FSDB {
        public static IFolderTable Open(string alias, string folderPath, string filter = null) => new FolderTable(new Alias(alias, folderPath, filter), Settings.EnableChecksums);        
        public static IFolderTable Open(Alias alias) => new FolderTable(alias, Settings.EnableChecksums);
    }
}