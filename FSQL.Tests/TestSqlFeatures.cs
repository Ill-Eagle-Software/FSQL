using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FSQL.Interfaces;
using FSQL.ProgramParts.FSQLStructs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSQL.Tests {
    [TestClass]
    public class TestSqlFeatures {
        public TestContext TestContext { get; set; }
        protected TestContext TestCtx => TestContext;



        [TestMethod]
        public void TestMapToStatement() {
            var expected = @"K:\Source\Antlr\AntlrTutorial\FSQL";
            var ctx = FSQLEngine.Create().Debug(@"
{
   MAP Source TO ""K:\\Source\\Antlr\\AntlrTutorial\\FSQL"";
   MAP Bin To ""K:\\Source\\Antlr\\AntlrTutorial\\FSQL\\bin"" WITH FILTER ""*.dll"";
}");
            Assert.IsNotNull(ctx);
            Assert.AreEqual(2, ctx.Aliases.Count());
            var actual = ctx.GetAlias("Source");
            Assert.AreEqual(expected, actual.Location);
        }


        [TestMethod]
        public void TestMapToStatementUsingExpressions()
        {
            var expected = @"K:\Source\Antlr\AntlrTutorial\FSQL";
            var expected2 = expected + @"\bin";

            var ctx = FSQLEngine.Create().Debug(@"
{
    @source = ""K:\\Source\\Antlr\\AntlrTutorial\\FSQL"";
    MAP Source TO @source; 
    @bin = ""K:\\Source\\Antlr\\AntlrTutorial\\FSQL\\bin"";
    @filter = ""*.dll"";
    MAP Bin To @bin WITH FILTER @filter;
}");
            Assert.IsNotNull(ctx);
            Assert.AreEqual(2, ctx.Aliases.Count());
            var actual = ctx.GetAlias("Source");
            Assert.AreEqual(expected, actual.Location);
            actual = ctx.GetAlias("Bin");
            Assert.AreEqual(expected2, actual.Location);
            Assert.AreEqual("*.dll", actual.FileSpec);
        }


        [TestMethod]
        public void TestUnMapStatement()
        {
            var expected = @"K:\Source\Antlr\AntlrTutorial\FSQL";
            var ctx = FSQLEngine.Create().Debug(@"
{
   MAP Source TO ""K:\\Source\\Antlr\\AntlrTutorial\\FSQL"";
   MAP Bin To ""K:\\Source\\Antlr\\AntlrTutorial\\FSQL\\bin"" WITH FILTER ""*.dll"";

   UNMAP Bin;

}");
            Assert.IsNotNull(ctx);
            Assert.AreEqual(1, ctx.Aliases.Count());
            var actual = ctx.GetAlias("Bin");
            Assert.IsNull(actual);

            actual = ctx.GetAlias("Source");
            Assert.AreEqual(expected, actual.Location);
        }




        [TestMethod]
        public void TestOpenTableFromAlias() {
            var expected = @".";
            var ctx = FSQLEngine.Create().Debug(@"
{
   MAP Source TO ""K:\\Source\\Antlr\\AntlrTutorial\\FSQL"";
   MAP Bin To ""."" WITH FILTER ""*.dll"";
}");
            Assert.IsNotNull(ctx);
            Assert.AreEqual(2, ctx.Aliases.Count());
            var src = ctx.GetAlias("Source");
            var bin = ctx.GetAlias("Bin");

            Assert.AreEqual(expected, bin.Location);

            var srcTable = src.Open();
            var binTable = bin.Open();

            foreach (var f in binTable.Rows) {
                Assert.IsTrue(f.Extension.ToLowerInvariant() == ".dll");
            }
        }



        [TestMethod]
        public void TestBasicSelect() {
            var expected = @".";
            var ctx = FSQLEngine.Create().Debug(@"
{   
   MAP Bin To ""."";
   return SELECT Bin.* FROM Bin FORMAT AS TEXT;
}");
            Assert.IsNotNull(ctx);

            var results = ctx.ReturnValue as string;
            Assert.IsNotNull(results);

            Console.WriteLine(results);

            Assert.AreEqual(1, ctx.Aliases.Count());
            var bin = ctx.GetAlias("Bin");

            Assert.AreEqual(expected, bin.Location);
            var binTable = bin.Open();            
        }

        [TestMethod]
        public void TestSelectWithSpecificAttributes()
        {
            var expected = @".";
            var ctx = FSQLEngine.Create().Debug(@"
{   
   MAP Bin To ""."" WITH FILTER ""*.dll"";

   @Results = SELECT FileName, Bin.Size AS FileLength, Created as CreatedOn, Bin.Updated AS LastModified FROM Bin;
   return @Results;

}");
            Assert.IsNotNull(ctx);

            var results = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(results);

            Assert.AreEqual(1, ctx.Aliases.Count());
            var bin = ctx.GetAlias("Bin");

            Assert.AreEqual(expected, bin.Location);
            var binTable = bin.Open();

            Assert.AreEqual(binTable.Rows.Count(), results.Rows.Count);
            
            results.WriteLine();
        }


        [TestMethod]
        public void TestSelectWithSpecificAttributesReorderingFields()
        {
            var expected = @".";
            var ctx = FSQLEngine.Create().Debug(@"
{   
   MAP Bin To ""."" WITH FILTER ""*.dll"";

   @Results = SELECT Bin.Size AS FileLength, Created as CreatedOn, Bin.Updated AS LastModified, NameOnly, Extension as Ext FROM Bin;
   return @Results;

}");
            Assert.IsNotNull(ctx);

            var results = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(results);

            Assert.AreEqual(1, ctx.Aliases.Count());
            var bin = ctx.GetAlias("Bin");

            Assert.AreEqual(expected, bin.Location);
            var binTable = bin.Open();

            Assert.AreEqual(binTable.Rows.Count(), results.Rows.Count);

            results.WriteLine();
        }

        [TestMethod]
        public void SelectWithAlternateWildcard()
        {
            var expected = @".";
            var ctx = FSQLEngine.Create().Debug(@"
{   
   TRACE ON;
   MAP Bin To ""."" WITH FILTER ""FSQL*.*"";

   @Results = SELECT Bin.Size AS FileLength, NameOnly, Extension as Ext FROM Bin;
   return @results; // Notice case insensitivity...

}");
            Assert.IsNotNull(ctx);

            var results = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(results);

            Assert.AreEqual(3, results.Columns.Count);

            Assert.AreEqual(1, ctx.Aliases.Count());
            var bin = ctx.GetAlias("Bin");

            Assert.AreEqual(expected, bin.Location);
            var binTable = bin.Open();

            Assert.AreEqual(binTable.Rows.Count(), results.Rows.Count);

            results.WriteLine();

            TestContext.WriteLine(ctx.ToString());
        }

        [TestMethod]
        public void TestContextTracing()
        {
            var expected = @".";
            var ctx = FSQLEngine.Create().Debug(@"
{   
    TRACE ON;    
    /* MAP Bin To ""."" WITH FILTER ""FSQL*.*""; */

    FUNC GetMyResults() {
        @Results = SELECT Bin.Size AS FileLength, NameOnly, Extension as Ext FROM ""."" Bin;        
        return @results;
    }

    WriteLine(""Retrieving data from Bin Folder..."");    
    @returnValue = GetMyResults();    
    TRACE OFF;
    return @returnValue; 

}");
            Assert.IsNotNull(ctx);

            var results = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(results);

            Assert.AreEqual(3, results.Columns.Count);

            //TestContext.WriteLine("TRACE DUMP");
            //TestContext.WriteLine("==========");
            //TestContext.WriteLine(ctx.ToString());
            TestContext.WriteLine(ctx.Dump(Priority.Verbose));

            Assert.AreEqual(1, ctx.Aliases.Count());
            var bin = ctx.GetAlias("Bin");

            Assert.AreEqual(expected, bin.Location);
            var binTable = bin.Open();

            Assert.AreEqual(binTable.Rows.Count(), results.Rows.Count);
        }

        [TestMethod]
        public void SelectWithAlternateWildcardToText()
        {
            //var expected = @".";
            var ctx = FSQLEngine.Create().Debug(@"
{   
   MAP Bin To ""."" WITH FILTER ""FSQL*.*"";

   @Results = SELECT Bin.Size AS FileLength, NameOnly, Extension as Ext FROM Bin FORMAT AS TEXT;
   return @results; // Notice case insensitivity...

}");
            Assert.IsNotNull(ctx);

            var results = ctx.ReturnValue as string;
            Assert.IsNotNull(results);

            Console.WriteLine(results);
        }

        [TestMethod]
        public void SelectWithAlternateWildcardToScalar()
        {
            
            var ctx = FSQLEngine.Create().Debug(@"
{  
   TRACE ON; 
   MAP F1 To "".\\Folder1"" WITH FILTER ""TargetFile1.txt"";

   @Results = SELECT F1.Size FROM F1 FORMAT AS SCALAR;
   return @results; // Notice case insensitivity...

}");
            Assert.IsNotNull(ctx);
            var expected = 25;
            var actual = Convert.ToInt32(ctx.ReturnValue);
            Console.WriteLine(actual);
            Console.WriteLine(ctx.ToString());
            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void SelectWithAlternateWildcardToJson()
        {
            //var expected = @".";
            var ctx = FSQLEngine.Create().Debug(@"
{   
   MAP Bin To ""."" WITH FILTER ""FSQL*.*"";

   @Results = SELECT Bin.Size AS FileLength, NameOnly, Extension as Ext FROM Bin FORMAT AS JSON;
   return @results; // Notice case insensitivity...

}");
            Assert.IsNotNull(ctx);

            var results = ctx.ReturnValue as string;
            Assert.IsNotNull(results);

            Console.WriteLine(results);
        }

        [TestMethod]
        public void SelectWithAlternateWildcardToCsv()
        {
            //var expected = @".";
            var ctx = FSQLEngine.Create().Debug(@"
{   
   MAP Bin To ""."" WITH FILTER ""FSQL*.*"";

   @Results = SELECT Bin.Size AS FileLength, NameOnly, Extension as Ext FROM Bin FORMAT AS CSV;
   return @results; // Notice case insensitivity...

}");
            Assert.IsNotNull(ctx);

            var results = ctx.ReturnValue as string;
            Assert.IsNotNull(results);

            Console.WriteLine(results);
        }

        [TestMethod]
        public void SelectWithInlineAlias()
        {
            var expected = @".";
            var ctx = FSQLEngine.Create().Debug(@"
{      
   @Results = SELECT Bin.Size AS FileLength, NameOnly, Extension as Ext 
                FROM ""."" Bin
              ;
   return @Results;

}");
            Assert.IsNotNull(ctx);

            var results = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(results);

            Assert.AreEqual(1, ctx.Aliases.Count());
            var bin = ctx.GetAlias("Bin");

            Assert.AreEqual(expected, bin.Location);
            var binTable = bin.Open();

            Assert.AreEqual(binTable.Rows.Count(), results.Rows.Count);

            results.WriteLine();
        }

        [TestMethod]
        public void TestOpenRowSet()
        {

            var ctx = FSQLEngine.Create().Debug(@"
{
   MAP Release TO ""..\\Release"" WITH FILTER ""*.*"";
   MAP Debug To ""."" WITH FILTER ""*.*"";
}");
            Assert.IsNotNull(ctx);
            Assert.AreEqual(2, ctx.Aliases.Count());
            var rel = ctx.GetAlias("Release");
            var dbg = ctx.GetAlias("Debug");

            var relTable = rel.Open();
            var dbgTable = dbg.Open();

            //relTable.GetData().WriteLine();
            //dbgTable.GetData().WriteLine();

            var results = relTable.Join(
                dbgTable, 
                (l, r) => l.FileName == r.FileName,
                ScopedAttribute.Create(new[]
                {
                    new[] { "Release", "FileName", "ReleaseFullName"},
                    new[] { "Release", "size", "ReleaseSize"},
                    new[] { "Debug", "size", "DebugSize"}
                }).ToArray()
            );

            results.WriteLine();

        }

        [TestMethod]
        public void TestJoinOperationUsingBin()
        {

            var ctx = FSQLEngine.Create().Debug(@"
{
   MAP Rel TO ""..\\Release"";
   MAP Dbg To ""."";

   return
   SELECT Rel.FileName, Dbg.Size as DebugSize, Rel.Size AS ReleaseSize
     FROM Rel JOIN Dbg ON Rel.FileName == Dbg.FileName AS CompareSizes
   ;

}");
            Assert.IsNotNull(ctx);

            var results = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(results);
            
            results.WriteLine();

            TestCtx.WriteLine(ctx.Dump(Priority.Verbose));

            Assert.AreEqual(3, results.Columns.Count);            
        }

        [TestMethod]
        public void TestJoinOperation()
        {

            var ctx = FSQLEngine.Create().Debug(@"
{
   TRACE ON;

   MAP F1 TO "".\\Folder1"";
   MAP F2 To "".\\Folder2"";

   return
   SELECT F1.FileName, F2.Size as DebugSize, F1.Size AS F1Size
     FROM F1 JOIN F2 ON F1.FileName == F2.FileName AS CompareSizes
   ;

}");
            Assert.IsNotNull(ctx);

            var results = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(results);

            results.WriteLine();

            if (ctx.TracingEnabled) TestCtx.WriteLine(ctx.Dump(Priority.Verbose));

            Assert.AreEqual(3, results.Columns.Count);
        }

        [TestMethod]
        public void TestOutputToScreen()
        {

            var ctx = FSQLEngine.Create().Debug(@"
{
   MAP Rel TO ""..\\Release"";
   MAP Dbg To ""."";

   @results =
   SELECT Rel.FileName as Name, Dbg.Size as Debug, Rel.Size AS Release
     FROM Rel JOIN Dbg ON Rel.FileName == Dbg.FileName AS CompareSizes
   OUTPUT TO SCREEN;

    return @results;

}", execCtx => execCtx.TracingEnabled = true);
            Assert.IsNotNull(ctx);

            var results = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(results);

            TestCtx.WriteLine(ctx.Dump(Priority.Verbose));

            Assert.AreEqual(3, results.Columns.Count);
        }

        [TestMethod]
        public void TestJsonOutputToClipboard()
        {

            var ctx = FSQLEngine.Create().Debug(@"
{
   MAP Rel TO ""..\\Release"";
   MAP Dbg To ""."";

   @results =
   SELECT Rel.FileName, Dbg.Size, Rel.Size AS ReleaseSize
     FROM Rel JOIN Dbg ON Rel.FileName == Dbg.FileName 
      AND NOT (Rel.Size == Dbg.Size) AS CompareSizes
   FORMAT AS JSON
   OUTPUT TO CLIPBOARD;

    return @results;

}");
            Assert.IsNotNull(ctx);

            var results = ctx.ReturnValue as string;
            
            Assert.IsNotNull(results);
            Console.WriteLine(results);

            var clipString = System.Windows.Forms.Clipboard.GetText();
            Assert.AreEqual(results, clipString);
            
        }

        [TestMethod]
        public void TestWriteToFileAndDeleteFile()
        {

            var ctx = FSQLEngine.Create().Debug(@"
{
    TRACE ON;
    @testsPassed = 0;

    func Check(@cond) {
        if (@cond) { @testsPassed = @testsPassed + 1; } 
        return @testsPassed;
    }

    @f1 = "".\\Folder1"";
    @f2 = "".\\Folder2"";

    @fName = ""F1.Files.Txt"";

    @targetFile = @f2 + ""\\"" + @fName;
    
    //Create File
    MAP F1 TO @f1;
    @files = SELECT F1.* FROM F1 FORMAT AS JSON OUTPUT TO FILE @targetFile;
    
    //Delete file
    MAP F2 TO @f2;
    @results = DELETE FROM F2 WHERE F2.FileName == @fName;

    @testsPassed = Check(@results == 1);

    return @testsPassed;

}");
            Assert.IsNotNull(ctx);
            if (ctx.TracingEnabled) TestCtx.WriteLine(ctx.Dump(Priority.Verbose).Replace("{", "{{").Replace("}", "}}"));

            var results = Convert.ToInt32(ctx.ReturnValue);

            Assert.AreEqual(1, results);
            Console.WriteLine(results.ToString());
        }

        [TestMethod]
        public void TestSubquery()
        {

            var ctx = FSQLEngine.Create().Debug(@"
{
   TRACE ON;

   MAP F1 TO "".\\Folder1"";
   MAP F2 To "".\\Folder2"";

   return
       SELECT F1.FileName as FolderOneOnly FROM F1
        WHERE F1.FileName NOT IN (SELECT F2.FileName FROM F2)
   ;     
   
}");
            //var expected = "TargetFile1";
            Assert.IsNotNull(ctx);
            if (ctx.TracingEnabled) TestCtx.WriteLine(ctx.Dump(Priority.Verbose));

            
            var actual = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(actual);

            Console.WriteLine($"File in Folder 1 But not Folder 2:");
            actual.WriteLine();

            
            // Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSimpleWhereClause() {

            var script = @"{ return SELECT F1.FileName, F1.Size FROM "".\\Folder1"" F1 WHERE F1.Size > 30; }";

            var ctx = FSQLEngine.Create().Debug(script, xCtx => xCtx.TracingEnabled = true);
            //var expected = "TargetFile1.txt";
            Assert.IsNotNull(ctx);
            if (ctx.TracingEnabled) TestCtx.WriteLine(ctx.Dump(Priority.Verbose));

            var actual = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(actual);

            Console.WriteLine($"File in Folder 1 With Size > 30:");
            actual.WriteLine();

            Assert.AreEqual(2, actual.Rows.Count);
   
            // Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSimpleWhereClause2()
        {

            var script = @"{ return SELECT F1.FileName, F1.Size FROM "".\\Folder1"" F1 WHERE F1.Size < 30; }";

            var ctx = FSQLEngine.Create().Debug(script, xCtx => xCtx.TracingEnabled = true);
            //var expected = "TargetFile1.txt";
            Assert.IsNotNull(ctx);
            if (ctx.TracingEnabled) TestCtx.WriteLine(ctx.Dump(Priority.Verbose));

            var actual = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(actual);

            Console.WriteLine($"File in Folder 1 With Size < 30:");
            actual.WriteLine();

            Assert.AreEqual(1, actual.Rows.Count);

            // Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSimpleWhereClause3()
        {

            var script = @"{ return SELECT F1.FileName, F1.Size FROM "".\\Folder1"" F1 WHERE F1.NameOnly == ""TargetFile1"" ; }";

            var ctx = FSQLEngine.Create().Debug(script, xCtx => xCtx.TracingEnabled = true);
            //var expected = "TargetFile1.txt";
            Assert.IsNotNull(ctx);
            if (ctx.TracingEnabled) TestCtx.WriteLine(ctx.Dump(Priority.Verbose));

            var actual = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(actual);

            Console.WriteLine($"File in Folder 1 With NameOnly = TargetFile1:");
            actual.WriteLine();

            Assert.AreEqual(1, actual.Rows.Count);

            // Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestChecksumClause()
        {

            var script = @"{ return SELECT F1.FileName, F1.Size, F1.CheckSum FROM "".\\Folder1"" F1 WHERE F1.Size > 30; }";

            var ctx = FSQLEngine.Create().Debug(script, xCtx => xCtx.TracingEnabled = true);
            //var expected = "TargetFile1.txt";
            Assert.IsNotNull(ctx);
            if (ctx.TracingEnabled) TestCtx.WriteLine(ctx.Dump(Priority.Verbose));

            var actual = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(actual);

            Console.WriteLine($"File in Folder 1 With Size > 30 (Including Checksums):");
            actual.WriteLine();

            Assert.AreEqual(2, actual.Rows.Count);

            // Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestJoinOnCheckSum()
        {

            var script = @"{ return 
                SELECT F1.FileName as F1Name, F2.FileName as F2Name, F1.CheckSum 
                  FROM "".\\Folder1"" F1 JOIN "".\\Folder2"" F2 ON F1.CheckSum == F2.CheckSum; 
            }";

            var ctx = FSQLEngine.Create().Debug(script, xCtx => xCtx.TracingEnabled = true);
            //var expected = "TargetFile1.txt";
            Assert.IsNotNull(ctx);
            if (ctx.TracingEnabled) TestCtx.WriteLine(ctx.Dump(Priority.Verbose));

            var actual = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(actual);

            Console.WriteLine($"File in Folder 1 With Size > 30 (Including Checksums):");
            actual.WriteLine();

            Assert.AreEqual(1, actual.Rows.Count);

            // Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestUseCheckSumToDetectModifiedFile()
        {

            var script = @"
            { 
                TRACE ON;
                @f1 = "".\\Folder1"";
                @f2 = "".\\Folder2"";

                Map F1 to @f1;
                Map F2 to @f2;

                return 
                SELECT F1.FileName as F1Name, F2.FileName as F2Name, F1.CheckSum 
                  FROM F1 
                  JOIN F2 
                    ON ((F1.FileName == F2.FileName) AND NOT (F1.CheckSum == F2.CheckSum)); 
            }";

            var ctx = FSQLEngine.Create().Debug(script, xCtx => xCtx.TracingEnabled = true);
            //var expected = "TargetFile1.txt";
            Assert.IsNotNull(ctx);
            if (ctx.TracingEnabled) TestCtx.WriteLine(ctx.Dump(Priority.Verbose));

            var actual = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(actual);

            Console.WriteLine($"File in Folder 1 With Size > 30 (Including Checksums):");
            actual.WriteLine();

            Assert.AreEqual(1, actual.Rows.Count);

            // Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestUseCheckSumToDetectModifiedFileWithoutReturningChecksum()
        {

            var script = @"
            { return 
                SELECT F1.FileName as F1Name, F2.FileName as F2Name 
                  FROM "".\\Folder1"" F1 
                  JOIN "".\\Folder2"" F2 
                    ON ((F1.FileName == F2.FileName) AND NOT (F1.CheckSum == F2.CheckSum)); 
            }";

            var ctx = FSQLEngine.Create().Debug(script, xCtx => xCtx.TracingEnabled = true);
            //var expected = "TargetFile1.txt";
            Assert.IsNotNull(ctx);
            if (ctx.TracingEnabled) TestCtx.WriteLine(ctx.Dump(Priority.Verbose));

            var actual = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(actual);

            Console.WriteLine($"File in Folder 1 With Size > 30 (Including Checksums):");
            actual.WriteLine();

            Assert.AreEqual(1, actual.Rows.Count);

            // Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSimulateStatement() {
            var script = @"
            { 
                @results = Simulate { 
                    @retValue = SELECT F1.FileName as F1Name, F2.FileName as F2Name, F1.CheckSum 
                                  FROM "".\\Folder1"" F1 
                                  JOIN "".\\Folder2"" F2 
                                    ON ((F1.FileName == F2.FileName) AND NOT (F1.CheckSum == F2.CheckSum)); 
                    return @retValue;
                };
                return @results;
            }";

            var ctx = FSQLEngine.Create().Debug(script, xCtx => xCtx.TracingEnabled = true);
            //var expected = "TargetFile1.txt";
            Assert.IsNotNull(ctx);
            if (ctx.TracingEnabled) TestCtx.WriteLine(ctx.Dump(Priority.Verbose));

            var actual = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(actual);

            Console.WriteLine($"File in Folder 1 With Size > 30 (Including Checksums):");
            actual.WriteLine();

            Assert.AreEqual(1, actual.Rows.Count);
        }


        [TestMethod]
        public void TestSimulatedSelectIntoStatement()
        {
            var script = @"
            { 
                return Sim { 
                    SELECT INTO "".\\Folder1"" F1
                      SELECT F2.FullName from "".\\Folder2"" F2;                                                        
                };                
            }";

            var ctx = FSQLEngine.Create().Debug(script, xCtx => xCtx.TracingEnabled = true);
            //var expected = "TargetFile1.txt";
            Assert.IsNotNull(ctx);
            if (ctx.TracingEnabled) TestCtx.WriteLine(ctx.Dump(Priority.Verbose));

            var actual = Convert.ToInt32(ctx.ReturnValue) ;
            Assert.AreEqual(3, actual);

        }

        [TestMethod]
        public void TestSimulatedInsertIntoStatement()
        {
            var script = @"
            { 
                return Sim { 
                    INSERT INTO "".\\Folder1"" F1
                      SELECT F2.FullName from "".\\Folder2"" F2;                                                        
                };                
            }";

            var ctx = FSQLEngine.Create().Debug(script, xCtx => xCtx.TracingEnabled = true);
            //var expected = "TargetFile1.txt";
            Assert.IsNotNull(ctx);
            if (ctx.TracingEnabled) TestCtx.WriteLine(ctx.Dump(Priority.Verbose));

            var actual = Convert.ToInt32(ctx.ReturnValue);
            Assert.AreEqual(3, actual);

        }


        [TestMethod]
        public void TestSelectIntoStatement()
        {
            var script = @"
            { 
                SELECT INTO "".\\Folder3"" F3
                    SELECT F2.FullName from "".\\Folder2"" F2;

                return SELECT * FROM F3; // Should see alias from query above.
                                
            }";

            var ctx = FSQLEngine.Create().Debug(script, xCtx => xCtx.TracingEnabled = true);
            //var expected = "TargetFile1.txt";
            Assert.IsNotNull(ctx);
            if (ctx.TracingEnabled) TestCtx.WriteLine(ctx.Dump(Priority.Verbose));

            var actual = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(actual);

            actual.WriteLine();

            Assert.AreEqual(3, actual.Rows.Count);
        }

        [TestMethod]
        public void TestDeleteFromStatement()
        {
            var script = @"
            { 
                MAP F2 TO "".\\Folder2"";
                MAP F3 TO "".\\Folder3"";                

                SELECT INTO F3
                    SELECT F2.FullName from F2;

                @allFiles = SELECT F3.FullName FROM F3 FORMAT AS CSV;
                WriteLine(@allFiles);

                DELETE FROM F3 WHERE NOT (F3.NAMEONLY == ""TargetFile1"");

                return SELECT F3.* FROM F3; 
                                
            }";

            var ctx = FSQLEngine.Create().Debug(script, xCtx => xCtx.TracingEnabled = true);
            //var expected = "TargetFile1.txt";
            Assert.IsNotNull(ctx);
            if (ctx.TracingEnabled) TestCtx.WriteLine(ctx.Dump(Priority.Verbose));

            var actual = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(actual);

            actual.WriteLine();

            Assert.AreEqual(1, actual.Rows.Count);
        }

        [TestMethod]
        public void TestSimulateDeleteFromStatement()
        {
            var script = @"
            { 
                SIMULATE {
                    DELETE FROM "".\\Folder2"" F2;
                }

                return SELECT * FROM F2; // Should see alias from query above.
                                
            }";

            var ctx = FSQLEngine.Create().Debug(script, xCtx => xCtx.TracingEnabled = true);
            //var expected = "TargetFile1.txt";
            Assert.IsNotNull(ctx);
            if (ctx.TracingEnabled) TestCtx.WriteLine(ctx.Dump(Priority.Verbose));

            var actual = ctx.ReturnValue as DataTable;
            Assert.IsNotNull(actual);

            actual.WriteLine();

            Assert.AreEqual(3, actual.Rows.Count);
        }

    }
}