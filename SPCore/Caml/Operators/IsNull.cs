using System;
using System.Xml.Linq;

namespace SPCore.Caml.Operators
{
    public sealed class IsNull : SingleFieldOperator
    {
        public IsNull(Guid fieldId)
            : base("IsNull", new FieldRef() { FieldId = fieldId })
        {
        }

        public IsNull(string fieldName)
            : base("IsNull", new FieldRef() { Name = fieldName })
        {
        }

        public IsNull(XElement existingIsNullOperator)
            : base("IsNull", existingIsNullOperator)
        {
        }
    }
}
