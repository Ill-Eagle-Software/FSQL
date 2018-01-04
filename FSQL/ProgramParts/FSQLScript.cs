using System;
using System.Diagnostics;
using FSQL.ExecCtx;
using FSQL.Interfaces;
using FSQL.ProgramParts.Core;

namespace FSQL.ProgramParts {
    public class FSQLScript : ProgramPart, IScript {
        private readonly Action<IExecutionContext> _initializer;
        private IExecutable Body { get; }

        public FSQLScript(IExecutable body, Action<IExecutionContext> contextInitializer = null) {
            _initializer = contextInitializer ?? (ctx => { });
            Body = body;
        }

        public override PartType Type { get; } = PartType.Script;

        public override void Dispose() {}

        public dynamic InvokeGeneric(IExecutionContext ctx, params object[] parms) {
            _initializer(ctx);
            dynamic retVal = (Body as IStatement)?.Execute(ctx, parms);
            return retVal;
        }

        public dynamic InvokeGeneric(params object[] parms) {
            var ctx = ExecutionContext.Create();
            return InvokeGeneric(ctx, parms);
        }

        public IExecutionContext Debug(IExecutionContext ctx, params object[] parms) {
            _initializer(ctx);
            dynamic retVal = (Body as IStatement)?.Execute(ctx, parms);
            return ctx;
        }

        public IExecutionContext Debug(params object[] parms) {
            var ctx = ExecutionContext.Create();
            return Debug(ctx, parms);
        }
    }
}