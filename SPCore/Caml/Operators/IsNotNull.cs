﻿using System;

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
    }
}
