using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FSQL.ExecCtx;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs {
    public class RowSet : ITable {

        protected const int Start = 0;
        protected int End => Data.Rows.Count;

        protected DataTable Data { get; set; }

        /// <summary>
        /// Use of this constructor REQUIRES the caller to instantiate the Data datatable.
        /// </summary>
        protected RowSet() {
            Current = new VariablesWrapper(vn => CurrentRow[vn], (vn, v) => CurrentRow[vn] = v);
        }

        public RowSet(DataTable data) : this(data.TableName, data)
        {
        }
        public RowSet(string tableName, DataTable data) {
            Data = data;            
            TableName = tableName;
            Current = new VariablesWrapper(vn => CurrentRow[vn], (vn, v) => CurrentRow[vn] = v);
        }

        public virtual string TableName { get; }
        public bool IsAtStart => Index == Start;
        public bool IsAtEnd => Index == End;
        public bool MoveFirst() => GoTo(Start);

        public bool MovePrev() => GoTo(Index - 1);

        public dynamic Current { get; }
        public int Index { get; protected set; }
        public virtual bool GoTo(int idx) {
            if (idx < Start || idx > End) return false;
            Index = idx;               
            return true;
        }

        public bool MoveNext() => GoTo(Index + 1);

        public bool MoveLast() => GoTo(End);

        public DataRow CurrentRow => IsAtEnd ? Data.NewRow() :  Data.Rows[Index];
        public virtual DataTable GetData() => Data;
        public virtual DataTable Filter(Func<dynamic, bool> condition, params ScopedAttribute[] selectedColumns)
        {
            var results = new DataTable("results");
            foreach (DataColumn dc in Data.Columns) results.Columns.Add(new DataColumn(dc.ColumnName));
            var thisIdx = Index;

            this.MoveFirst();
            do {
                if (condition(this.Current)) {
                    var colIdx = 0;
                    var row = results.NewRow();
                    foreach (DataColumn col in Data.Columns) {
                        row[colIdx++] = CurrentRow[col.ColumnName];
                    }
                    results.Rows.Add(row);
                }
            } while (this.MoveNext());

            results = results.ReduceColumnsToAliases(selectedColumns);
            //FinalizeColumns(results, selectedColumns);

            GoTo(thisIdx);            
            return results;
        }

        public virtual DataTable Filter(Func<DataRow, bool> condition)
        {
            var results = new DataTable("results");
            foreach (DataColumn dc in Data.Columns) results.Columns.Add(new DataColumn(dc.ColumnName));
            var thisIdx = Index;

            this.MoveFirst();
            do
            {
                if (condition(CurrentRow))
                {
                    var colIdx = 0;
                    var row = results.NewRow();
                    foreach (DataColumn col in Data.Columns)
                    {
                        row[colIdx++] = CurrentRow[col.ColumnName];
                    }
                    results.Rows.Add(row);
                }
            } while (this.MoveNext() && !IsAtEnd);

            GoTo(thisIdx);
            return results;
        }

        public virtual DataTable Join(IActiveRecord that, Func<dynamic, dynamic, bool> condition, params ScopedAttribute[] selectedColumns)
        {
            var results = BuildMergeResultsSet(that);

            var thisIdx = Index;
            var thatIdx = that.Index;

            DataRow[] leftData = new DataRow[GetData().Rows.Count],
                      rightData = new DataRow[(that.GetData().Rows.Count)];

            GetData().Rows.CopyTo(leftData,0);
            that.GetData().Rows.CopyTo(rightData, 0);

            var data = from leftRow in leftData
                       from rightRow in rightData
                       let left = new VariablesWrapper(vn => leftRow[vn], (vn, v) => leftRow[vn] = v)
                       let right = new VariablesWrapper(vn => rightRow[vn], (vn, v) => rightRow[vn] = v)
                       where condition(left, right)
                       select BuildMergeResultRow(results, leftRow, rightRow);

            foreach (var row in data) results.Rows.Add(row);

            //selectedColumns = selectedColumns?.SelectFrom(n => n.ToLowerInvariant());
            results = results.ReduceColumnsToAliases(selectedColumns);
            //FinalizeColumns(results, selectedColumns);

            GoTo(thisIdx);
            that.GoTo(thatIdx);
            return results;
        }

        protected virtual DataTable BuildMergeResultsSet(IActiveRecord that) {
            var col = that.GetData().Columns;
            var results = new DataTable();
            foreach (DataColumn c in Data.Columns) results.Columns.Add(new DataColumn($"{this.TableName}.{c.ColumnName}"));
            foreach (DataColumn c in col) results.Columns.Add(new DataColumn($"{that.TableName}.{c.ColumnName}"));
            return results;
        }

        public DataRow BuildMergeResultRow(DataTable tbl, DataRow left, DataRow right)
        {
            var colIdx = 0;
            var srcIdx = 0;
            var row = tbl.NewRow();
            while (srcIdx < left.ItemArray.Length)
            {
                row[colIdx++] = left[srcIdx++];
            }
            srcIdx = 0;
            while (srcIdx < right.ItemArray.Length)
            {
                row[colIdx++] = right[srcIdx++];
            }
            return row;
        }
    }
}