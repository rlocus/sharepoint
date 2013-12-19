﻿using System;
using System.Xml.Linq;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public sealed class BeginsWith : SingleFieldValueOperator<string>
    {
        public BeginsWith(Guid fieldId, string value)
            : base("BeginsWith", fieldId, value, SPFieldType.Text)
        {
        }

        public BeginsWith(string fieldName, string value)
            : base("BeginsWith", fieldName, value, SPFieldType.Text)
        {
        }

        public BeginsWith(string existingBeginsWithOperator)
            : base("BeginsWith", existingBeginsWithOperator)
        {
        }

        public BeginsWith(XElement existingBeginsWithOperator)
            : base("BeginsWith", existingBeginsWithOperator)
        {
        }
    }
}
