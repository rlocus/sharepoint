using System;
using System.Xml.Linq;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public sealed class Gt<T> : SingleFieldValueOperator<T>
    {
        public Gt(Guid fieldId, T value, SPFieldType type)
            : base("Gt", fieldId, value, type)
        {
        }

        public Gt(string fieldName, T value, SPFieldType type)
            : base("Gt", fieldName, value, type)
        {
        }

        public Gt(string existingGtOperator)
            : base("Gt", existingGtOperator)
        {
        }

        public Gt(XElement existingGtOperator)
            : base("Gt", existingGtOperator)
        {
        }
    }
}
