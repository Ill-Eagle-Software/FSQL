using System;
using System.IO;
using FSQL.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSQL.Tests {
    [TestClass]
    public class TestFunctionFeatures {
        public TestContext TestContext { get; set; }
        protected TestContext TestCtx => TestContext;

        [TestMethod]
        public void TestBooleanReturnStatement() {
            var expected = true;
            var actual = FSQLEngine.Create().Execute("{ return true; return false; }");
            Assert.AreEqual(expected, actual);

            expected = false;
            actual = FSQLEngine.Create().Execute("{ return false; return true; }");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIntegerReturnStatement() {
            var expected = 31;
            var actual = FSQLEngine.Create().Execute("{ return 31; return 42; }");
            Assert.AreEqual(expected, actual);

            expected = 78;
            actual = FSQLEngine.Create().Execute("{ return 78; return 61; }");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestInputOutputStatements()
        {
            

            var expected = "Hello!";
            var script = @"
{
    WriteLine(""Enter 'Hello!' and press ENTER:"");
    @var = ReadLine();
    Write(""You entered: "", @var);
    WriteLine();
    return @var;
}";

            object actual;
            using (StringReader sr = new StringReader("Hello!\n")) {
                Console.SetIn(sr);
                actual = FSQLEngine.Create().Execute(script);

            }

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestFunction() {
            var script = @"
{
    FUNC myfunction() {

        FUNC subFunction() {
            @temp = 7;
            @temp2 = 7;
            return @temp * 2;
        }

        @tempvar = subFunction();
        return @tempvar;
    }

    return myFunction();
}
";
            var expected = 14;
            var actual = FSQLEngine.Create().Execute(script);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParameters()
        {
            var script = @"
{
    FUNC myfunction() {

        FUNC mult(@left, @right) {
            return @left * @right;
        }

        @tempvar = mult(2, 7);
        return @tempvar;
    }

    return myFunction();
}
";
            var expected = 14;
            var actual = FSQLEngine.Create().Execute(script);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCoreDumpSystemFunction()
        {
            var script = @"
{
    TRACE ON;
    @topLevelVariable = 5;
    FUNC myfunction() {

        FUNC subFunction() {
            return CoreDump();
        }
        
        @tempvar = subFunction();
        WriteLine();
        WriteLine(""==========================="");
        WriteLine(@tempvar);
        WriteLine(""==========================="");
        WriteLine();
        return @tempvar;
    }

    return myFunction();
}
";
            IExecutionContext ctx = null;
            object actual = null;
            try {
                ctx = FSQLEngine.Create().Debug(script);
                actual = ctx.ReturnValue;
            } finally {
                Console.WriteLine(ctx.Dump(Priority.Verbose));
                //TestCtx.WriteLine(actual.ToString());
            }
        }

        [TestMethod]
        public void TestCoreDumpSystemFunction2()
        {
            var script = @"
{
    @topLevelVariable = 5;
    FUNC myfunction() {

        FUNC subFunction() {
            return CoreDump();
        }
        @runfunc = subFunction();
        @tempvar = CoreDump(); 
        return @tempvar;
    }

    return myFunction();
}
";
            var actual = FSQLEngine.Create().Execute(script) as string;
            TestCtx.WriteLine(actual);
            Assert.IsTrue(actual.Contains("myfunction"));
            Assert.IsFalse(actual.Contains("topLevelVariable"));
        }

        [TestMethod]
        public void TestFunctionParameters() {
            var script = @"
{
    FUNC add(@left, @right) {
        @results = @left + @right;
        return @results;
    }

    return add(2, 4);
}
";
            var ctx = FSQLEngine.Create().Debug(script);
            var actual = Convert.ToInt32(ctx.ReturnValue);
            Console.WriteLine(ctx.Dump(Priority.Verbose));
            Assert.AreEqual(6, actual);                    
        }

        [TestMethod]
        public void TestFunctionsAsParameters()
        {
            var script = @"
{
    FUNC add(@left, @right) {
        @results = @left + @right;
        return @results;
    }

    return add(add(1,1), add(1,3));
}
";
            var actual = Convert.ToInt32(FSQLEngine.Create().Execute(script));
            Assert.AreEqual(6, actual);
            Console.WriteLine(actual);
        }

        [TestMethod]
        public void TestExportStatement() {
            var script = @"
{
    Trace On;

    func testExport() {
        @exported = 14; // This variable will be exported to the calling context.
        @notExported = 35;
        export @exported;
        return @notExported;
    }

    @throwaway=testExport(); // Run the function.
    return @exported;
}
";
            var expected = 14;
            var ctx = FSQLEngine.Create().Debug(script);
            var actual = ctx.ReturnValue;
            TestCtx.WriteLine(ctx.Dump(Priority.Verbose));
            Assert.AreEqual(expected, actual);
            
        }

        [TestMethod]
        public void TestPrivateVariables()
        {
            var script = @"
{
    Trace On;

    @notExported = 65;

    func testExport() {
        @exported = 35; // This variable will be exported to the calling context.
        @notExported = 80;
        export @exported;
        return @notExported == 80;
    }

    @fnResults=testExport(); // Run the function.
    WriteLine(""@fnResults should be TRUE."");
    return @notExported;
}
";
            var expected = 65;
            var ctx = FSQLEngine.Create().Debug(script);
            var actual = ctx.ReturnValue;
            TestCtx.WriteLine(ctx.Dump(Priority.Verbose));
            Assert.AreEqual(expected, actual);
            //TestCtx.WriteLine(ctx["fnresults"].ToString());
            //TestCtx.WriteLine(ctx["fnresults"].GetType().Name);
            Assert.IsTrue((bool) ctx["fnresults"]);
        }


        [TestMethod]
        public void TestRecursiveFunction()
        {
            var script = @"
{
    Trace On;
    Func Factorial(@num) {
        if (@num == 0) {
            @results = 1;
        } else {
            @prev = Factorial(@num-1);
            @results = @num * @prev;
        }
        return @results;
    }
    return Factorial(5);
}
";
            var expected = 5*4*3*2;
            var ctx = FSQLEngine.Create().Debug(script);
            var actual = (int) ctx.ReturnValue;
            var dump = ctx.Dump(Priority.Verbose);
            TestCtx.WriteLine(dump.Replace("{", "{{").Replace("}", "}}"));
            Assert.AreEqual(expected, actual);

        }

    }
}