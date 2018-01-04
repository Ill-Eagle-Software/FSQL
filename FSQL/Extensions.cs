using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSQL.ProgramParts.FSQLStructs;

namespace FSQL
{
    public static class Extensions
    {
        public static void WriteLine(this DataTable dt)
        {

            const int colWidth = -25;
            foreach (DataColumn c in dt.Columns)
            {
                Console.Write($"{c.ColumnName,colWidth}|");
            }
            Console.WriteLine();
            foreach (DataColumn c in dt.Columns)
            {
                Console.Write($"{new string('=', Math.Abs(colWidth)),colWidth}+");
            }
            Console.WriteLine();

            foreach (DataRow r in dt.Rows)
            {
                var colIdx = 0;
                foreach (DataColumn c in dt.Columns)
                {
                    Console.Write($"{r[colIdx++],colWidth}|");
                }
                Console.WriteLine();
            }
        }

        internal static DataTable ReduceColumnsToAliases(this DataTable source, IEnumerable<ScopedAttribute> selectedColumns) {
            if (selectedColumns == null || !selectedColumns.Any()) return source;
            var results = BuildTableFromAttributes(selectedColumns);

            foreach (DataRow srcRow in source.Rows) {
                var newRow = results.NewRow();
                foreach (DataColumn col in results.Columns) {
                    var alias = col.ColumnName;
                    var columnDef = selectedColumns.First(c => c.ColumnName == alias);
                    //var srcColName  // columnDef.CanonicalName;
                    var value = GetValue(srcRow, columnDef);
                    newRow[alias] = value;
                }
                results.Rows.Add(newRow);
            }

            return results;
        }

        private static dynamic GetValue(DataRow source, ScopedAttribute attr) {
            if (source.Table.Columns.Contains(attr.CanonicalName)) {
                return source[attr.CanonicalName];
            } else {
                return source[attr.PropertyName];
            }
        }

        private static DataTable BuildTableFromAttributes(IEnumerable<ScopedAttribute> columns) {
            var resultTable = new DataTable();
            foreach (var col in columns) {
                resultTable.Columns.Add(new DataColumn(col.ColumnName));
            }
            return resultTable;
        }

        internal static string Unescape(this string source)
        {
            return source.Trim(new char[] { '\"' })
                .Replace("\\t", "\t")
                .Replace("\\\\", "\\")
                .Replace("\\n", "\n")
                .Replace("\\r", "\r");
        }

    }
}
