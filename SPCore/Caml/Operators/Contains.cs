﻿using System;
using System.Xml.Linq;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public sealed class Contains : SingleFieldValueOperator<string>
    {
        public Contains(FieldRef fieldRef, Value<string> value)
            : base("Contains", fieldRef, value)
        {
        }

        public Contains(FieldRef fieldRef, string value)
            : base("Contains", fieldRef, value, SPFieldType.Text)
        {
        }

        public Contains(Guid fieldId, string value)
            : base("Contains", fieldId, value, SPFieldType.Text)
        {
        }

        public Contains(string fieldName, string value)
            : base("Contains", fieldName, value, SPFieldType.Text)
        {
        }

        public Contains(string existingContainsOperator)
            : base("Contains", existingContainsOperator)
        {
        }

        public Contains(XElement existingContainsOperator)
            : base("Contains", existingContainsOperator)
        {
        }
    }
}
