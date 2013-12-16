using System;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public sealed class Gt<T> : SingleFieldValueOperator<T>
    {
        public Gt(Guid fieldId, T value, SPFieldType type)
            : base("Gt", fieldId, value, type)
        {
        }

        public Gt(string fieldName, T value, SPFieldType type)
            : base("Gt", fieldName, value, type)
        {
        }
    }
}
