using System;
using System.Xml.Linq;

namespace SPCore.Caml.Operators
{
    public sealed class IsNotNull : SingleFieldOperator
    {
        public IsNotNull(Guid fieldId)
            : base("IsNotNull", new FieldRef() { FieldId = fieldId })
        {
        }

        public IsNotNull(string fieldName)
            : base("IsNotNull", new FieldRef() { Name = fieldName })
        {
        }

        public IsNotNull(XElement existingIsNotNullOperator)
            : base("IsNotNull", existingIsNotNullOperator)
        {
        }
    }
}
