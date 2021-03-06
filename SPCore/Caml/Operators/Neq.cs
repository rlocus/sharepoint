﻿using System;
using System.Xml.Linq;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public sealed class Neq<T> : SingleFieldValueOperator<T>
    {
        public Neq(FieldRef fieldRef, Value<T> value)
            : base("Neq", fieldRef, value)
        {
        }

        public Neq(FieldRef fieldRef, T value, SPFieldType type)
            : base("Neq", fieldRef, value, type)
        {
        }

        public Neq(Guid fieldId, T value, SPFieldType type)
            : base("Neq", fieldId, value, type)
        {
        }

        public Neq(string fieldName, T value, SPFieldType type)
            : base("Neq", fieldName, value, type)
        {
        }

        public Neq(string existingNeqOperator)
            : base("Neq", existingNeqOperator)
        {
        }

        public Neq(XElement existingNeqOperator)
            : base("Neq", existingNeqOperator)
        {
        }
    }
}
