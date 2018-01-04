using System;
using System.Data;
using System.Diagnostics;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs {
    public class DataRowFileInfo : IFileInformation {

        private readonly DataRow _myData;

        public DataRowFileInfo(DataRow data) {
            FullName = data[FolderTable.FullName].ToString();
            Path = data[FolderTable.PathField].ToString();
            Name = data[FolderTable.FileName].ToString();
            NameWithoutExtension = data[FolderTable.FileNameOnly].ToString();
            Extension = data[FolderTable.FileExtension].ToString();
            Size = data.IsNull(FolderTable.Size) ? 0L : (long) data[FolderTable.Size];
            CreatedUtc = data.IsNull(FolderTable.CreatedUtc) ? DateTime.MinValue : (DateTime) data[FolderTable.CreatedUtc];
            ModifiedUtc = data.IsNull(FolderTable.ModifiedUtc) ? DateTime.MinValue : (DateTime) data[FolderTable.ModifiedUtc];
            _myData = data;            
        }

        public string FullName { get; }
        public string Path { get; }
        public string Name { get; }
        public string NameWithoutExtension { get; }
        public string Extension { get; }
        public long Size { get; }
        public DateTime CreatedUtc { get; }
        public DateTime ModifiedUtc { get; }

        public static implicit operator DataRowFileInfo (DataRow dr) => new DataRowFileInfo(dr);
        public static implicit operator DataRow(DataRowFileInfo drfi) => drfi._myData;

    }
}