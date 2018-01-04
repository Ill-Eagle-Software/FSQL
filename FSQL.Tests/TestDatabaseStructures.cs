using System;
using System.Linq;
using System.Threading.Tasks;
using FSQL.ProgramParts;
using FSQL.ProgramParts.FSQLStructs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSQL.Tests
{
    [TestClass]
    public class TestDatabaseStructures
    {
        public TestContext TestContext { get; set; }
        protected TestContext ctx => TestContext;

        [TestMethod]
        public async Task TestFolderTable() {
            var thisTable = FSDB.Open("Current", ".");
            foreach (var row in thisTable.Rows) {
                ctx.WriteLine($"{row.Name,-30}{row.CreatedUtc:dd-MMM-yyyy hh:mm:ss}  {row.Path,-80}");
            }
            var rows = await thisTable.GetRowsAsync();
            Assert.IsTrue(rows.Any(r => r.Name == "FSQL.exe"));
            Assert.IsTrue(rows.Any(r => r.Name == "FSQL.Tests.dll"));
        }

        [TestMethod]
        public async Task TestFolderTableWithFilter()
        {
            var thisTable = FSDB.Open(new Alias("Current", ".", "*.exe"));
            var rows = await thisTable.GetRowsAsync();
            foreach (var row in thisTable.Rows)
            {
                ctx.WriteLine($"{row.Name,-30}{row.CreatedUtc:dd-MMM-yyyy hh:mm:ss}  {row.Path,-80}");
            }
            Assert.IsTrue(thisTable.Rows.Any(r => r.Name == "FSQL.exe"));
            Assert.IsTrue(thisTable.Rows.All(r => r.Name != "FSQL.Tests.dll"));
        }
    }
}
