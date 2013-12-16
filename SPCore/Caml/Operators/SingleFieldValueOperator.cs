using System;
using System.Xml.Linq;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public abstract class SingleFieldValueOperator<T> : ValueOperator<T>
    {
        public FieldRef FieldRef { get; set; }

        protected SingleFieldValueOperator(string operatorName, Guid fieldId, T value, SPFieldType type)
            : base(operatorName, value, type)
        {
            FieldRef = new FieldRef() { FieldId = fieldId };
        }

        protected SingleFieldValueOperator(string operatorName, string fieldName, T value, SPFieldType type)
            : base(operatorName, value, type)
        {
            FieldRef = new FieldRef() { Name = fieldName };
        }

        public override XElement ToXElement()
        {
            var ele = base.ToXElement();
            ele.Add(FieldRef.ToXElement());
            return ele;
        }
    }
}
