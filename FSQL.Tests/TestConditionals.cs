using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSQL.Tests {
    [TestClass]
    public class TestConditionals {
        public TestContext TestContext { get; set; }
        protected TestContext TestCtx => TestContext;

        [TestMethod]
        public void TestIfTrueStatement()
        {
            var script = @"
{
    @candy = 45;
    if (true) {
        @candy = 80;
    } else {
        @candy = 60;
    }

    return @candy;
}
";
            var expected = 80;
            var actual = FSQLEngine.Create().Execute(script);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIfFalseStatement()
        {
            var script = @"
{
    @candy = 45;
    if (false) {
        @candy = 80;
    } else {
        @candy = 60;
    }

    return @candy;
}
";
            var expected = 60;
            var actual = FSQLEngine.Create().Execute(script);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIfTrueWithoutElseStatement()
        {
            var script = @"
{
    @candy = 45;
    if (true) {
        @candy = 80;
    } 

    return @candy;
}
";
            var expected = 80;
            var actual = FSQLEngine.Create().Execute(script);
            Assert.AreEqual(expected, actual);
        }
   

        [TestMethod]
        public void TestIfFalseWithoutElseStatement()
        {
            var script = @"
{
    @candy = 45;
    if (false) {
        @candy = 80;
    } 

    return @candy;
}
    ";
            var expected = 45;
            var actual = FSQLEngine.Create().Execute(script);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestWhileDoStatement()
        {
            var script = @"
{
    @candy = -3;
    While (5 > @candy) {
        @candy = @candy + 1;
    } 

    return @candy;
}
    ";
            var expected = 5;
            var actual = FSQLEngine.Create().Execute(script);
            Assert.AreEqual(expected, actual);
        }

    }
}