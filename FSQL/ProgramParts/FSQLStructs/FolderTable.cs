using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Schema;
using FSQL.Exceptions;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs {
    public class FolderTable : RowSet, IFolderTable, ICached {

        protected ICacheControl Cache { get; }

        public FolderTable(Alias alias, bool enableChecksums, bool flatten = false) {
            Alias = alias;
            EnableChecksums = enableChecksums;
            Flattened = flatten;
            Data = new DataTable(Alias.Name);
            Data.Columns.AddRange(GetStructure());
            Cache = new CacheControl(this);
        }

        private Alias Alias { get; }
        public bool EnableChecksums { get; set; }
        public string Name => Alias.Name;
        public string Location => Alias.Location;
        public string FileSpec => Alias.FileSpec;
        public bool IsFolder => Alias.IsFolder;
        public bool IsFile => Alias.IsFile;
        public bool Exists => Alias.Exists;

        public IFolderTable Open() => this;

        public IEnumerable<IFileInformation> Rows {
            get {
                Cache.RefreshAsync().Wait();
                if (Flattened) {
                    throw new NotImplementedException("Flattened tables have not been implemented.");
                } else {
                    foreach (DataRow r in Data.Rows) {
                        yield return new DataRowFileInfo(r);
                    }
                }
            }
        }

        public async Task<IEnumerable<IFileInformation>> GetRowsAsync() {
            await Cache.RefreshAsync();
            var results = new List<IFileInformation>();            
            foreach (DataRow r in Data.Rows)
            {
                results.Add(new DataRowFileInfo(r));
            }
            return results;
        }

        public IFileInformation CurrentRecord { get; protected set; }

        public bool Flattened { get; }

        private DataColumn[] GetStructure() {
            var LONG = typeof(long);
            var DATE = typeof(DateTime);
            var dcs = new List<DataColumn>
            {
                Field(FullName),
                Field(PathField),
                Field(FileName),
                Field(FileNameOnly),
                Field(FileExtension),
                Field(Size, LONG),
                Field(CreatedUtc, DATE),
                Field(ModifiedUtc, DATE),
                Field(CheckSum)
            };
            return dcs.ToArray();
        }

        private DataRow Row(string fullname, long size, DateTime created, DateTime modified, string checkSum) {
            var dr = Data.NewRow();
            dr[FullName] = fullname;
            dr[PathField] = Path.GetFullPath(fullname);
            dr[FileName] = Path.GetFileName(fullname);
            dr[FileNameOnly] = Path.GetFileNameWithoutExtension(fullname);
            dr[FileExtension] = Path.GetExtension(fullname);
            dr[Size] = size;
            dr[CreatedUtc] = created;
            dr[ModifiedUtc] = modified;
            dr[CheckSum] = checkSum;
            return dr;
        }

        private DataColumn Field(string name, Type datatype = null) {
            datatype = datatype ?? typeof(string);
            return new DataColumn(name, datatype);
        }

        internal const string FullName = "FullName";
        internal const string PathField = "Path";
        internal const string FileName = "FileName";
        internal const string FileNameOnly = "NameOnly";
        internal const string FileExtension = "Extension";
        internal const string Size = "Size";
        internal const string CreatedUtc = "Created";
        internal const string ModifiedUtc = "Updated";
        internal const string CheckSum = "CheckSum";

        #region Cache Interface

        bool ICached.OnRead() => DoOnRead();

        async Task<bool> ICached.OnReadAsync()
        {
            var results = await Task.Run(() => DoOnRead());
            return results;
        }

        protected bool DoOnRead() {
            Data.Rows.Clear();
            if (Location == null) throw new UndefinedAliasException($"Alias [{Name}] is undefined or the referenced folder, [{Location}] does not exist.");
            foreach (var fullname in Directory.GetFiles(Location, FileSpec, SearchOption.TopDirectoryOnly))
            {
                var fi = new FileInfo(fullname);
                var size = fi.Length;
                var created = fi.CreationTimeUtc;
                var modified = fi.LastWriteTimeUtc;
                var checksum = EnableChecksums ? GetChecksum(fullname) : string.Empty;
                Data.Rows.Add(Row(fullname, size, created, modified, checksum));
            }
            if (Index >= Data.Rows.Count) MoveLast();
            return true;
        }


        protected static string GetChecksum(string file)
        {
            var results = string.Empty;
            using (var stream = File.OpenRead(file))
            {
                results = GetChecksumBuffered(stream);
                //var sha = new SHA256Managed();
                //byte[] checksum = sha.ComputeHash(stream);
                //return BitConverter.ToString(checksum).Replace("-", String.Empty);
            }
            return results;
        }

        protected static string GetChecksumBuffered(Stream stream)
        {
            using (var bufferedStream = new BufferedStream(stream, 1024 * 32))
            {
                var sha = new SHA256Managed();
                var checksum = sha.ComputeHash(bufferedStream);
                return BitConverter.ToString(checksum).Replace("-", String.Empty);
            }
        }
        #endregion

        public override DataTable GetData() {
            Cache.RefreshAsync().Wait();
            return base.GetData();
        }

        public override bool GoTo(int idx) {
            Cache.RefreshAsync().Wait();
            if (base.GoTo(idx)) {
                CurrentRecord = new DataRowFileInfo(CurrentRow);
                return true;
            }
            return false;
        }

        public override string TableName => Alias.Name;

    }


}