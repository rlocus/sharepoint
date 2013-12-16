using System;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public sealed class Lt<T> : SingleFieldValueOperator<T>
    {
        public Lt(Guid fieldId, T value, SPFieldType type)
            : base("Lt", fieldId, value, type)
        {
        }

        public Lt(string fieldName, T value, SPFieldType type)
            : base("Lt", fieldName, value, type)
        {
        }
    }
}
