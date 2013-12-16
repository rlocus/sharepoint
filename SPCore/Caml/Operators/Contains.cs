using System;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public sealed class Contains : SingleFieldValueOperator<string>
    {
        public Contains(Guid fieldId, string value)
            : base("Contains", fieldId, value, SPFieldType.Text)
        {
        }

        public Contains(string fieldName, string value)
            : base("Contains", fieldName, value, SPFieldType.Text)
        {
        }
    }
}
