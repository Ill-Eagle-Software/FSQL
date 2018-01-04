using System.Collections.Generic;
using FSQL.ExecCtx;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSQL.Tests {
    [TestClass]
    public class TestExecutionContext {
        public TestContext TestContext { get; set; }
        protected TestContext TestCtx => TestContext;

        [TestMethod]
        public void TestExecutionContextChaining() {
            // ROOT CONTEXT
            var ctx = ExecutionContext.Create();
            TestCtx.WriteLine($"Level {ctx.Level}:: {ctx.ContextPath}");
            Assert.IsNull(ctx.Parent);
            Assert.IsTrue(ctx.IsRoot);

            ctx["name"] = "Terry Lewis";
            ctx["age"] = 49;

            Assert.AreEqual("Terry Lewis", ctx["name"]);
            Assert.AreEqual(49, ctx["age"]);

            // FIRST SPAWNED CONTEXT
            ctx = ctx.Enter("SpawnedContext", new Dictionary<string, object> {{"isTesting", true}});
            TestCtx.WriteLine($"Level {ctx.Level}:: {ctx.ContextPath}");
            Assert.IsNotNull(ctx.Parent);
            Assert.IsTrue((bool) ctx["isTesting"]);
            Assert.IsFalse(ctx.IsRoot);
            Assert.AreEqual("SpawnedContext", ctx["__method"]);
            Assert.AreEqual("Terry Lewis", ctx["name"]);
            Assert.AreEqual(49, ctx["age"]);

            ctx["name"] = "Bo Duke";
            Assert.AreEqual("Bo Duke", ctx["name"]);
            Assert.AreNotEqual("Terry Lewis", ctx["name"]);
            Assert.AreEqual(49, ctx["age"]);

            ctx = ctx.Exit();
            TestCtx.WriteLine($"Level {ctx.Level}:: {ctx.ContextPath}");
            Assert.IsNull(ctx.Parent);
            Assert.IsTrue(ctx.IsRoot);
            Assert.AreEqual("Terry Lewis", ctx["name"]);
            Assert.AreEqual(49, ctx["age"]);
            Assert.IsFalse((bool) ctx["isTesting"]);

            // SECOND SPAWNED CONTEXT
            ctx = ctx.Enter("SpawnedContext2", new Dictionary<string, object> { { "isTesting", true } });
            TestCtx.WriteLine($"Level {ctx.Level}:: {ctx.ContextPath}");
            Assert.IsTrue((bool)ctx["isTesting"]);
            Assert.IsNotNull(ctx.Parent);
            Assert.IsFalse(ctx.IsRoot);
            Assert.AreEqual("SpawnedContext2", ctx["__method"]);
            Assert.AreEqual("Terry Lewis", ctx["name"]);
            Assert.AreEqual(49, ctx["age"]);

            ctx["name"] = "Luke Duke";
            Assert.AreEqual("Luke Duke", ctx["name"]);
            Assert.AreNotEqual("Terry Lewis", ctx["name"]);
            Assert.AreEqual(49, ctx["age"]);

            // Export name variable to parent context.
            Assert.AreEqual("Luke Duke", ctx.Export("name"));

            ctx = ctx.Exit();
            Assert.IsNull(ctx.Parent);
            Assert.IsTrue(ctx.IsRoot);
            Assert.AreNotEqual("Terry Lewis", ctx["name"]);
            Assert.AreEqual("Luke Duke", ctx["name"]);
            Assert.AreEqual(49, ctx["age"]);

        }


        [TestMethod]
        public void TestDynamicVariableInterface()
        {
            // ROOT CONTEXT
            var ctx = ExecutionContext.Create();
            TestCtx.WriteLine($"Level {ctx.Level}:: {ctx.ContextPath}");
            Assert.IsNull(ctx.Parent);
            Assert.IsTrue(ctx.IsRoot);

            ctx.Variables.name = "Terry Lewis";
            ctx.Variables.age = 49;

            Assert.AreEqual("Terry Lewis", ctx["name"]);
            Assert.AreEqual("Terry Lewis", ctx.Variables.name);
            Assert.AreEqual(49, ctx["age"]);
            Assert.AreEqual(49, ctx.Variables.age);

            // FIRST SPAWNED CONTEXT
            ctx = ctx.Enter("SpawnedContext", new Dictionary<string, object> { { "isTesting", true } });
            TestCtx.WriteLine($"Level {ctx.Level}:: {ctx.ContextPath}");
            Assert.IsNotNull(ctx.Parent);
            Assert.IsTrue((bool)ctx["isTesting"]);
            Assert.IsFalse(ctx.IsRoot);
            Assert.AreEqual("SpawnedContext", ctx.Variables.__method);
            Assert.AreEqual("Terry Lewis", ctx["name"]);
            Assert.AreEqual("Terry Lewis", ctx.Variables.name);
            Assert.AreEqual(49, ctx["age"]);
            Assert.AreEqual(49, ctx.Variables.age);

            ctx.Variables.name = "Bo Duke";
            Assert.AreEqual("Bo Duke", ctx["name"]);
            Assert.AreNotEqual("Terry Lewis", ctx["name"]);
            Assert.AreEqual("Bo Duke", ctx.Variables.name);
            Assert.AreEqual(49, ctx["age"]);

            ctx = ctx.Exit();
            TestCtx.WriteLine($"Level {ctx.Level}:: {ctx.ContextPath}");
            Assert.AreEqual("Terry Lewis", ctx["name"]);
            Assert.AreEqual("Terry Lewis", ctx.Variables.name);
            Assert.AreEqual(49, ctx["age"]);
            Assert.AreEqual(49, ctx.Variables.age);

            // SECOND SPAWNED CONTEXT
            ctx = ctx.Enter("SpawnedContext2", new Dictionary<string, object> { { "isTesting", true } });
            TestCtx.WriteLine($"Level {ctx.Level}:: {ctx.ContextPath}");
            Assert.IsTrue((bool)ctx["isTesting"]);
            Assert.IsNotNull(ctx.Parent);
            Assert.IsFalse(ctx.IsRoot);
            Assert.AreEqual("SpawnedContext2", ctx["__method"]);
            Assert.AreEqual("Terry Lewis", ctx["name"]);
            Assert.AreEqual(49, ctx["age"]);

            ctx.Variables.name = "Luke Duke";
            Assert.AreEqual("Luke Duke", ctx["name"]);
            Assert.AreNotEqual("Terry Lewis", ctx["name"]);
            Assert.AreEqual("Luke Duke", ctx.Variables.name);
            Assert.AreEqual(49, ctx["age"]);

            // Export name variable to parent context.
            Assert.AreEqual("Luke Duke", ctx.Export("name"));

            ctx = ctx.Exit();
            Assert.IsNull(ctx.Parent);
            Assert.IsTrue(ctx.IsRoot);
            Assert.AreNotEqual("Terry Lewis", ctx.Variables.name);
            Assert.AreEqual("Luke Duke", ctx.Variables.name);
            Assert.AreEqual(49, ctx["age"]);

        }
    }
}