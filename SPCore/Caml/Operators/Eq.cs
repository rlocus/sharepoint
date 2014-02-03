using System;
using System.Xml.Linq;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public sealed class Eq<T> : SingleFieldValueOperator<T>
    {
        public Eq(FieldRef fieldRef, Value<T> value)
            : base("Eq", fieldRef, value)
        {
        }

        public Eq(FieldRef fieldRef, T value, SPFieldType type)
            : base("Eq", fieldRef, value, type)
        {
        }

        public Eq(Guid fieldId, T value, SPFieldType type)
            : base("Eq", fieldId, value, type)
        {
        }

        public Eq(string fieldName, T value, SPFieldType type)
            : base("Eq", fieldName, value, type)
        {
        }

        public Eq(string existingEqOperator)
            : base("Eq", existingEqOperator)
        {
        }

        public Eq(XElement existingEqOperator)
            : base("Eq", existingEqOperator)
        {
        }
    }
}
