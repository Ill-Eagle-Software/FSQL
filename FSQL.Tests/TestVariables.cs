using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSQL.Tests {
    [TestClass]
    public class TestVariables {
        public TestContext TestContext { get; set; }
        protected TestContext TestCtx => TestContext;

        [TestMethod]
        public void SetAndRetrieveVariableViaScript()
        {
            var expected = 42;
            var actual = FSQLEngine.Create().Execute(@"
{
   @answer=41; 
   @answer=@answer+5;
   @answer=@answer-4; 
   @answer=@answer*2; 
   @answer=@answer/2; 
   return @answer; 
}");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CalculateLeahsAge()
        {
            var expected = 13;
            var actual = FSQLEngine.Create().Execute(@"
{
   @currentyear=2017;
   @leahwasbornin=2004;
   @leahsage=@currentyear-@leahwasbornin; 
   return @leahsage; 
}");
            Assert.AreEqual(expected, actual);
        }
    }
}