using System;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public sealed class Eq<T> : SingleFieldValueOperator<T>
    {
        public Eq(Guid fieldId, T value, SPFieldType type)
            : base("Eq", fieldId, value, type)
        {
        }

        public Eq(string fieldName, T value, SPFieldType type)
            : base("Eq", fieldName, value, type)
        {
        }
    }
}
