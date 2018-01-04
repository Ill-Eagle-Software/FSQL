using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSQL.Tests {
    [TestClass]
    public class TestComplexExpressions {
        public TestContext TestContext { get; set; }
        protected TestContext TestCtx => TestContext;

        [TestMethod]
        public void EvaluateOrderOfOperations() {
            Assert.AreEqual(0D, FSQLEngine.Create().Execute("{return 1+2-3;}"));
            Assert.AreEqual(2D, FSQLEngine.Create().Execute("{return 2*4-6;}"));
            Assert.AreEqual(-2D, FSQLEngine.Create().Execute("{return 6-4*2;}"));
            Assert.AreEqual(4D, FSQLEngine.Create().Execute("{return (6-4)*2;}"));
        }
    }
}