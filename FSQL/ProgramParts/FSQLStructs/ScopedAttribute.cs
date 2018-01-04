using System.Collections.Generic;
using System.Linq;
using FSQL.Exceptions;
using FSQL.Interfaces;
using FSQL.ProgramParts.Core;
using FSQL.ProgramParts.FSQLStructs.SelectObjects;

namespace FSQL.ProgramParts.FSQLStructs {
    public class ScopedAttribute : ProgramPart, IExpression
    {
        private readonly string _columnName;

        public static ScopedAttribute Create(string objectName, string propertyName, string columnName) {
            return new ScopedAttribute(objectName, propertyName, columnName);
        }
        public static IEnumerable<ScopedAttribute> Create(params string[][] data) =>
            data.Select(def => new ScopedAttribute(def[0], def[1], def[2]));
        

        public ScopedAttribute(string objectName, string propertyName, string columnName) {
            ObjectName = objectName;
            PropertyName = propertyName;
            _columnName = columnName;
        }
        public string ObjectName { get; }
        public string PropertyName { get; }
        public string CanonicalName => ObjectName != null ? $"{ObjectName}.{PropertyName}" : PropertyName;
        public string ColumnName => _columnName ?? PropertyName;

        public override PartType Type { get; } = PartType.Expression;
        public override void Dispose() {}

        public override string ToString() => CanonicalName + (ColumnName == PropertyName ? "" : $" AS {ColumnName}");
        public object Execute(IExecutionContext ctx, params object[] parms) {

            var aliasVariableName = $"{Constants.TablePrefix}{ObjectName}";

            var rSet = ctx[aliasVariableName] as RowSet;
            var table = rSet.GetData();
            dynamic value;
            if (table.Columns.Contains(CanonicalName))
            {
                value = rSet.CurrentRow[CanonicalName];
            }
            else if (table.Columns.Contains(ColumnName))
            {
                value = rSet.CurrentRow[ColumnName];
            }
            else
            {
                throw new ColumnResolutionException($"Could not resolve column '{CanonicalName}' to retrieve its value.");
            }
            return value;
        }

        public object GetGenericValue(IExecutionContext ctx) => Execute(ctx);
    }

}