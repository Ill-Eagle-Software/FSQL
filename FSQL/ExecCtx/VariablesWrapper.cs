using System;
using System.Dynamic;

namespace FSQL.ExecCtx {
    public class VariablesWrapper : DynamicObject {

        protected Func<string, object> GetVariable;
        protected Action<string, object> SetVariable;

        public VariablesWrapper(Func<string, object> getVariable, Action<string, object> setVariable) {
            GetVariable = getVariable;
            SetVariable = setVariable;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            result = GetVariable(binder.Name);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value) {
            SetVariable(binder.Name, value);
            return true;
        }
    }
}