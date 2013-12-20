using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint;
using SPCore.Caml.Interfaces;

namespace SPCore.Caml.Operators
{
    public abstract class SingleFieldValueOperator<T> : ValueOperator<T>, ISingleFieldOperator
    {
        public FieldRef FieldRef { get; set; }

        protected SingleFieldValueOperator(string operatorName, FieldRef fieldRef, Value<T> value)
            : base(operatorName, value)
        {
            FieldRef = fieldRef;
        }

        protected SingleFieldValueOperator(string operatorName, FieldRef fieldRef, T value, SPFieldType type)
            : base(operatorName, value, type)
        {
            FieldRef = fieldRef;
        }

        protected SingleFieldValueOperator(string operatorName, Guid fieldId, Value<T> value)
            : base(operatorName, value)
        {
            FieldRef = new FieldRef() { FieldId = fieldId };
        }

        protected SingleFieldValueOperator(string operatorName, Guid fieldId, T value, SPFieldType type)
            : base(operatorName, value, type)
        {
            FieldRef = new FieldRef() { FieldId = fieldId };
        }

        protected SingleFieldValueOperator(string operatorName, string fieldName, Value<T> value)
            : base(operatorName, value)
        {
            FieldRef = new FieldRef() { Name = fieldName };
        }

        protected SingleFieldValueOperator(string operatorName, string fieldName, T value, SPFieldType type)
            : base(operatorName, value, type)
        {
            FieldRef = new FieldRef() { Name = fieldName };
        }

        protected SingleFieldValueOperator(string operatorName, string existingSingleFieldValueOperator)
            : base(operatorName, existingSingleFieldValueOperator)
        {
        }

        protected SingleFieldValueOperator(string operatorName, XElement existingSingleFieldValueOperator)
            : base(operatorName, existingSingleFieldValueOperator)
        {
        }

        protected override void OnParsing(XElement existingSingleFieldValueOperator)
        {
            XElement existingFieldRef = existingSingleFieldValueOperator.Elements().SingleOrDefault(el => string.Equals(el.Name.LocalName, "FieldRef", StringComparison.InvariantCultureIgnoreCase));

            if (existingFieldRef != null)
            {
                FieldRef = new FieldRef(existingFieldRef);
            }

            XElement existingValue = existingSingleFieldValueOperator.Elements().SingleOrDefault(el => el.Name.LocalName == "Value");

            if (existingValue != null)
            {
                base.OnParsing(existingValue);
            }
        }

        public override XElement ToXElement()
        {
            XElement el = base.ToXElement();
            if (FieldRef != null) el.AddFirst(FieldRef.ToXElement());
            return el;
        }
    }
}
