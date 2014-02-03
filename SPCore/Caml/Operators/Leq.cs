using System;
using System.Xml.Linq;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public sealed class Leq<T> : SingleFieldValueOperator<T>
    {
        public Leq(FieldRef fieldRef, Value<T> value)
            : base("Leq", fieldRef, value)
        {
        }

        public Leq(FieldRef fieldRef, T value, SPFieldType type)
            : base("Leq", fieldRef, value, type)
        {
        }

        public Leq(Guid fieldId, T value, SPFieldType type)
            : base("Leq", fieldId, value, type)
        {
        }

        public Leq(string fieldName, T value, SPFieldType type)
            : base("Leq", fieldName, value, type)
        {
        }

        public Leq(string existingLeqOperator)
            : base("Leq", existingLeqOperator)
        {
        }

        public Leq(XElement existingLeqOperator)
            : base("Gt", existingLeqOperator)
        {
        }
    }
}
