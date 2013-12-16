using System;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public sealed class Leq<T> : SingleFieldValueOperator<T>
    {
        public Leq(Guid fieldId, T value, SPFieldType type)
            : base("Leq", fieldId, value, type)
        {
        }

        public Leq(string fieldName, T value, SPFieldType type)
            : base("Leq", fieldName, value, type)
        {
        }
    }
}
