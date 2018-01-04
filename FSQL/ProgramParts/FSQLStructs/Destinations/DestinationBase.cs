using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs.Destinations {
    public abstract class DestinationBase : IExpression {

        private readonly IExpression _dataRetriever;

        protected DestinationBase(IExpression dataRetriever) {
            _dataRetriever = dataRetriever;
        }
        public PartType Type { get; }
        public object Execute(IExecutionContext ctx, params object[] parms) {
            var data = _dataRetriever.GetGenericValue(ctx);
            OnDataReceived(ctx, data);
            return data;
        }

        public object GetGenericValue(IExecutionContext ctx) => Execute(ctx);

        protected abstract void OnDataReceived(IExecutionContext ctx, object data);

        public override string ToString() => _dataRetriever.ToString();

        public void Dispose() {
            _dataRetriever.Dispose();
        }
    }
}