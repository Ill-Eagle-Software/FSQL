using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using FSQL.Interfaces;
using FSQL.ProgramParts.Core;
using FSQL.ProgramParts.FSQLStructs.Sources;
using FSQL.ProgramParts.FSQLStructs.WhereClauses;

namespace FSQL.ProgramParts.FSQLStructs.SelectObjects {
    public class DeleteFrom : StatementBase, IExpression {
        private readonly AliasRef _target;
        private readonly WhereClauseBase _where;

        public DeleteFrom(AliasRef target, WhereClauseBase where) {
            _target = target;
            _where = where;
        }
        public object GetGenericValue(IExecutionContext ctx) => OnExecute(ctx);

        protected override object OnExecute(IExecutionContext ctx, params object[] parms) {
            var isSim = ctx.InSimulationMode;
            _where.Initialize(ctx);
            var files = GetFilesToDelete(ctx);
            var filesDeleted = 0;
            var delete = isSim ? (Action<IExecutionContext, string>) SimulateDeleteFile : DeleteFile;
            foreach (var file in files) {
                delete(ctx, file);
                filesDeleted++;
            }
            return filesDeleted;
        }

        protected void DeleteFile(IExecutionContext ctx, string filename) {
            ctx.Trace($"rm '{filename}'.");
            File.Delete(filename);
        }

        protected void SimulateDeleteFile(IExecutionContext ctx, string filename) => ctx.Trace($"SIM: rm '{filename}'.");

        protected IEnumerable<string> GetFilesToDelete(IExecutionContext ctx) {
            var results = new List<string>();
            var sqlCtx = ctx.Enter("__DeleteFrom");
            try {
                var source = new SingleSourceAggregator(_target);

                var fTable = source.GetValue(sqlCtx);

                var table = (_where is NullWhereClause)
                    ? fTable.GetData()
                    : fTable.Filter((DataRow dr) => _where.Test(sqlCtx, dr));

                foreach (DataRow r in table.Rows) {
                    results.Add(r[FolderTable.FullName].ToString());
                }
            } finally {
                sqlCtx?.Exit(results);
            }
            return results;

        }

        public override void Dispose() {
            _target.Dispose();
            _where.Dispose();
        }
    }
}