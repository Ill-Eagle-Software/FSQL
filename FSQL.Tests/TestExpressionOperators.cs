using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSQL.Tests {
    [TestClass]
    public class TestExpressionOperators {
        public TestContext TestContext { get; set; }
        protected TestContext ctx => TestContext;

        #region Strings
        [TestMethod]
        public void EvaluateStringConcat() {
            var source = "{ return \"Hello, \" + \"World!\";} ";
            const string expected = "Hello, World!";
            var actual = FSQLEngine.Create().Execute(source);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region Mathematics

        [TestMethod]
        public void EvaluateInteger() {
            Assert.AreEqual(5, FSQLEngine.Create().Execute("return 5"));
        }

        [TestMethod]
        public void EvaluateNegativeInteger() {
            Assert.AreEqual(-15D, FSQLEngine.Create().Execute("return -15"));
        }

        [TestMethod]
        public void EvaluateNumber() {
            Assert.AreEqual(5.4, FSQLEngine.Create().Execute("return 5.4"));
        }

        [TestMethod]
        public void EvaluateIntPower() {
            Assert.AreEqual(25D, FSQLEngine.Create().Execute("return {return 5^2;}"));
        }

        [TestMethod]
        public void EvaluateNumberPower() {
            Assert.AreEqual(0.25D, FSQLEngine.Create().Execute("return {return 0.5^2.0;}"));
        }

        [TestMethod]
        public void EvaluateIntegerMultAndDivision() {
            Assert.AreEqual(25D, FSQLEngine.Create().Execute("return { return 5*5; }"));
            Assert.AreEqual(15D, FSQLEngine.Create().Execute("return { return 30/2; }"));
        }

        [TestMethod]
        public void EvaluateIntegerAddAndSubtract() {
            Assert.AreEqual(10D, FSQLEngine.Create().Execute("return {return 5+5;}"));
            //Assert.AreEqual(10D, FSQLEngine.Create().Execute("return 30/2"));
        }

        #endregion

        #region Booleans

        [TestMethod]
        public void EvaluateBooleanStatements() {
            Assert.IsTrue((bool) FSQLEngine.Create().Execute("{return TRUE;}"));
            Assert.IsTrue((bool) FSQLEngine.Create().Execute("{return NOT FALSE;}"));

            Assert.IsFalse((bool) FSQLEngine.Create().Execute("{return FALSE;}"));
            Assert.IsFalse((bool) FSQLEngine.Create().Execute("{return NOT TRUE;}"));
        }

        [TestMethod]
        public void EvaluateBooleanOrStatements() {
            Assert.IsTrue((bool) FSQLEngine.Create().Execute("{return TRUE OR TRUE;}"));
            Assert.IsTrue((bool) FSQLEngine.Create().Execute("{return TRUE OR FALSE;}"));
            Assert.IsTrue((bool) FSQLEngine.Create().Execute("{return FALSE OR TRUE;}"));
            Assert.IsFalse((bool) FSQLEngine.Create().Execute("{return FALSE OR FALSE;}"));

        }

        [TestMethod]
        public void EvaluateBooleanAndStatements() {
            Assert.IsTrue((bool) FSQLEngine.Create().Execute("{return TRUE AND TRUE;}"));
            Assert.IsFalse((bool) FSQLEngine.Create().Execute("{return TRUE AND FALSE;}"));
            Assert.IsFalse((bool) FSQLEngine.Create().Execute("{return FALSE AND TRUE;}"));
            Assert.IsFalse((bool) FSQLEngine.Create().Execute("{return FALSE AND FALSE;}"));
        }

        [TestMethod]
        public void EvaluateBooleanParentheticalStatements() {
            Assert.IsTrue((bool) FSQLEngine.Create().Execute("{return TRUE AND (TRUE OR FALSE);}"));
            Assert.IsTrue((bool)FSQLEngine.Create().Execute("{return TRUE OR (TRUE AND FALSE);}"));
            Assert.IsFalse((bool)FSQLEngine.Create().Execute("{return TRUE AND (TRUE AND FALSE);}"));
            //Assert.IsFalse((bool)FSQLEngine.Create().Execute("return FALSE AND FALSE"));
        }

        #endregion

    }
}
