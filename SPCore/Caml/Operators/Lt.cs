using System;
using System.Xml.Linq;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public sealed class Lt<T> : SingleFieldValueOperator<T>
    {
        public Lt(Guid fieldId, T value, SPFieldType type)
            : base("Lt", fieldId, value, type)
        {
        }

        public Lt(string fieldName, T value, SPFieldType type)
            : base("Lt", fieldName, value, type)
        {
        }

        public Lt(string existingLtOperator)
            : base("Lt", existingLtOperator)
        {
        }

        public Lt(XElement existingLtOperator)
            : base("Lt", existingLtOperator)
        {
        }
    }
}
