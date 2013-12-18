using System;
using System.Xml.Linq;
using Microsoft.SharePoint;
using SPCore.Caml.Interfaces;

namespace SPCore.Caml.Operators
{
    public abstract class SingleFieldValueOperator<T> : ValueOperator<T>, ISingleFieldOperator
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
            XElement el = base.ToXElement();
            if (FieldRef != null) el.AddFirst(FieldRef.ToXElement());
            return el;
        }
    }
}
