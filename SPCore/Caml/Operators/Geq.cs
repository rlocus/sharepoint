using System;
using System.Xml.Linq;
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

        public Geq(string existingGeqOperator)
            : base("Geq", existingGeqOperator)
        {
        }

        public Geq(XElement existingGeqOperator)
            : base("Geq", existingGeqOperator)
        {
        }
    }
}
