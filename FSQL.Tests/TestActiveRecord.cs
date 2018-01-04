using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FSQL.ProgramParts.FSQLStructs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSQL.Tests {
    [TestClass]
    public class TestActiveRecord
    {
        public TestContext TestContext { get; set; }
        protected TestContext TestCtx => TestContext;

        [TestMethod]
        public void TestDataTableWrapper() {
            var ds = GetDataTable001();
            var wrap = new RowSet(ds);

            Assert.IsTrue(wrap.MoveFirst()); // Move to first record
            Assert.AreEqual(wrap.Current.FirstName, "Terry");
            Assert.AreEqual(wrap.Current.MiddleName, "L.");
            Assert.AreEqual(wrap.Current.LastName, "Lewis");

            Assert.IsTrue(wrap.MoveNext()); // Move to next record
            Assert.AreEqual(wrap.Current.FirstName, "Dawn");
            Assert.AreEqual(wrap.Current.Age, 46);

            Assert.IsTrue(wrap.MoveNext()); // Move to next record
            Assert.AreEqual(wrap.Current.FirstName, "Bethany");
            Assert.AreEqual(wrap.Current.Age, 21);

            Assert.IsTrue(wrap.MoveNext()); // Move to next record
            Assert.AreEqual(wrap.Current.FirstName, "Leah");
            Assert.AreEqual(wrap.Current.Age, 13);

            Assert.IsTrue(wrap.MoveNext()); // Can move to next record...

            Assert.IsTrue(wrap.IsAtEnd);

        }


        [TestMethod]
        public void TestForStatementWithActiveRecord()
        {
            var ds = GetDataTable001();
            var wrap = new RowSet(ds);

            var recCount = 0;
            for (wrap.MoveFirst(); !wrap.IsAtEnd; wrap.MoveNext()) {
                Console.WriteLine($"{++recCount}: {wrap.Current.FirstName}");
            }

            Assert.AreEqual(4, recCount);
        }

        [TestMethod]
        public void TestDataTableWrapperJoin()
        {
            
            var wrap = new RowSet(GetDataTable001());
            var that = new RowSet(GetDataTable002());
            var ds = wrap.Join(that, (l, r) => l.FirstName == r.FirstName);
            Assert.AreEqual(4, ds.Rows.Count);
            Assert.AreEqual(6, ds.Columns.Count);

            ds.WriteLine();
        }

        [TestMethod]
        public void TestColumnFiltering()
        {
            var wrap = new RowSet(GetDataTable001());
            var that = new RowSet(GetDataTable002());

            var ds = wrap.Join(that, (l, r) => l.FirstName == r.FirstName, 
                ScopedAttribute.Create(new[]
            {
                new[] {"age", "firstname", "FirstName"},
                new[] {"age", "middlename", "MiddleInitial"},
                new[] {"age", "lastname", "LastName"},
                new[] {"age", "age", "Age"},
                new[] { "hobby", "hobby", "Hobby"}
            }).ToArray());
            Assert.AreEqual(4, ds.Rows.Count);
            Assert.AreEqual(5, ds.Columns.Count);
            ds.WriteLine();
        }

        private DataTable GetDataTable001() {
            var results = new DataTable();
            results.TableName = "Age";
            results.Columns.Add(new DataColumn("FirstName", typeof(string)));
            results.Columns.Add(new DataColumn("MiddleName", typeof(string)));
            results.Columns.Add(new DataColumn("LastName", typeof(string)));
            results.Columns.Add(new DataColumn("Age", typeof(int)));

            var row = results.NewRow();
            row["FirstName"] = "Terry";
            row["MiddleName"] = "L.";
            row["LastName"] = "Lewis";
            row["Age"] = 49;
            results.Rows.Add(row);

            row = results.NewRow();
            row["FirstName"] = "Dawn";
            row["MiddleName"] = "C.";
            row["LastName"] = "Lewis";
            row["Age"] = 46;
            results.Rows.Add(row);

            row = results.NewRow();
            row["FirstName"] = "Bethany";
            row["MiddleName"] = "M.";
            row["LastName"] = "Lewis";
            row["Age"] = 21;
            results.Rows.Add(row);

            row = results.NewRow();
            row["FirstName"] = "Leah";
            row["MiddleName"] = "G.";
            row["LastName"] = "Lewis";
            row["Age"] = 13;
            results.Rows.Add(row);

            return results;
        }

        private DataTable GetDataTable002() {
            var results = new DataTable("Hobby");
            results.Columns.Add(new DataColumn("FirstName", typeof(string)));
            results.Columns.Add(new DataColumn("Hobby", typeof(string)));

            var row = results.NewRow();
            row["FirstName"] = "Terry";
            row["Hobby"] = "Reading";
            results.Rows.Add(row);

            row = results.NewRow();
            row["FirstName"] = "Dawn";
            row["Hobby"] = "Singing";
            results.Rows.Add(row);

            row = results.NewRow();
            row["FirstName"] = "Bethany";
            row["Hobby"] = "Color Guard";
            results.Rows.Add(row);

            row = results.NewRow();
            row["FirstName"] = "Leah";
            row["Hobby"] = "Cheer";
            results.Rows.Add(row);

            return results;
        }

    }
}