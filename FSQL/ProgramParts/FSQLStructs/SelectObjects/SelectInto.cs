using System;
using System.Data;
using System.IO;
using System.Text;
using FSQL.Exceptions;
using FSQL.Interfaces;
using FSQL.ProgramParts.Core;

namespace FSQL.ProgramParts.FSQLStructs.SelectObjects {
    public class SelectInto : StatementBase {
        private readonly AliasRef _target;
        private readonly SelectFrom _source;
        private readonly bool _canCreateTarget;

        public SelectInto(AliasRef target, SelectFrom source, bool canCreateTarget) {
            _target = target;
            _source = source;
            _canCreateTarget = canCreateTarget;
        }

        public override void Dispose() {}

        protected override object OnExecute(IExecutionContext ctx, params object[] parms) {
            var isSimulation = ctx.InSimulationMode;

            var createFolder = isSimulation ? (Action <IExecutionContext, string>) SimulateCreateFolder : CreateFolder;
            var copyFile = isSimulation ? (Action<IExecutionContext, string, string>) SimulateCopyFile : CopyFile;
            
            // Does target exist?
            var alias = _target.GetValue(ctx);
            if (!Directory.Exists(alias.Location)) { 
                if (_canCreateTarget)
                    createFolder(ctx, alias.Location);
                else
                    throw new TargetFolderDoesNotExistException($"Folder '{alias.Location}' does not exist. Use SELECT INTO if you want the folder to be created automaticaly.");
            }

            var dt = _source.GetGenericValue(ctx) as DataTable;

            var getSourceFilePath = GetPathFunction(dt);

            if (getSourceFilePath == null) {
                throw new MissingRequiredColumnException((_canCreateTarget ? "SELECT INTO" : "INSERT INTO") + " cannot determine how to retrieve the file path from the subquery. " +
                    "The subquery must include one of the following sets of columns: [FullName], [Path, FileName], or [Path, NameOnly, Extension].");
            }

            int filesCopied = 0;

            foreach (DataRow dr in dt.Rows) {
                var source = getSourceFilePath(dr);
                var fn = Path.GetFileName(source);
                var dest = Path.Combine(alias.Location, fn);
                copyFile(ctx, source, dest);
                filesCopied++;
            }

            return filesCopied;
        }

        protected virtual Func<DataRow, string> GetPathFunction(DataTable dt) {
            if (dt.Columns.Contains("FullName")) return (dr => dr["FullName"].ToString());

            if (dt.Columns.Contains("Path") &&
                dt.Columns.Contains("FileName")
                ) return (dr => Path.Combine(dr["Path"].ToString(), dr["FileName"].ToString()));

            if (dt.Columns.Contains("Path") &&
                dt.Columns.Contains("NameOnly") &&
                dt.Columns.Contains("Extension")                
                ) return (dr => Path.Combine(dr["Path"].ToString(), dr["NameOnly"].ToString() + dr["Extension"].ToString()));

            return null;
        }

        protected void SimulateCreateFolder(IExecutionContext ctx, string path) => ctx.Trace($"SIM: Creating folder '{path}'.");

        protected void CreateFolder(IExecutionContext ctx, string path) {
            ctx.Trace($"Creating folder '{path}'.");
            Directory.CreateDirectory(path);
        }

        protected void SimulateCopyFile(IExecutionContext ctx, string source, string dest) => ctx.Trace($"SIM: cp '{source}' '{dest}'");

        protected void CopyFile(IExecutionContext ctx, string source, string dest) {
            ctx.Trace($"cp '{source}' '{dest}'");
            File.Copy(source, dest, true);
        }

        public override string ToString() {
            var sb = new StringBuilder(_canCreateTarget ? "SELECT INTO " : "INSERT INTO ");
            sb.Append(_target);
            sb.Append(" ");
            sb.Append(_source);
            return sb.ToString();
        }
    }
}