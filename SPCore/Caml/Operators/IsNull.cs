using System;

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
    }
}
