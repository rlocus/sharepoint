using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public sealed class In<T> : SingleFieldMultipleValueOperator<T>
    {
        public In(Guid fieldId, IEnumerable<Value<T>> values)
            : base("In", fieldId, values)
        {
        }

        public In(Guid fieldId, IEnumerable<T> values, SPFieldType type)
            : base("In", fieldId, values, type)
        {
        }

        public In(string fieldName, IEnumerable<T> values, SPFieldType type)
            : base("In", fieldName, values, type)
        {
        }

        public In(string fieldName, IEnumerable<Value<T>> values)
            : base("In", fieldName, values)
        {
        }

        public In(FieldRef fieldRef, IEnumerable<T> values, SPFieldType type)
            : base("In", fieldRef, values, type)
        {
        }

        public In(FieldRef fieldRef, IEnumerable<Value<T>> values)
            : base("In", fieldRef, values)
        {
        }

        public In(string existingInOperator)
            : base("In", existingInOperator)
        {
        }

        public In(XElement existingInOperator)
            : base("In", existingInOperator)
        {
        }
    }
}
