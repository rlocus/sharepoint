using System;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public sealed class Geq<T> : SingleFieldValueOperator<T>
    {
        public Geq(Guid fieldId, T value, SPFieldType type)
            : base("Geq", fieldId, value, type)
        {
        }

        public Geq(string fieldName, T value, SPFieldType type)
            : base("Geq", fieldName, value, type)
        {
        }
    }
}
