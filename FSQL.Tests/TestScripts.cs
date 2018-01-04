using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSQL.Tests {
    [TestClass]
    public class TestScripts
    {
        public TestContext TestContext { get; set; }
        protected TestContext TestCtx => TestContext;

        [TestMethod]
        public void EvaluateIfStatement() {
            var script = @"
{
    @test=40;
    if (@test == 40) {
        @test = @test + 1;
    } else {
        @test = @test + 2;
    }
    return @test;
}
";

            var expected = 41;
            var actual = FSQLEngine.Create().Execute(script);
            
            TestCtx.WriteLine($" RESULTS: {actual}");            
            Assert.AreEqual(expected, actual);            
        }

    }
}