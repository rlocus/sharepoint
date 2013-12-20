using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint;
using SPCore.Caml.Interfaces;

namespace SPCore.Caml.Operators
{
    public abstract class SingleFieldMultipleValueOperator<T> : MultipleValueOperator<T>, ISingleFieldOperator
    {
        public FieldRef FieldRef { get; set; }

        protected SingleFieldMultipleValueOperator(string operatorName, Guid fieldId, IEnumerable<Value<T>> values)
            : base(operatorName, values)
        {
            FieldRef = new FieldRef() { FieldId = fieldId };
        }

        protected SingleFieldMultipleValueOperator(string operatorName, Guid fieldId, IEnumerable<T> values, SPFieldType type)
            : base(operatorName, values, type)
        {
            FieldRef = new FieldRef() { FieldId = fieldId };
        }

        protected SingleFieldMultipleValueOperator(string operatorName, string fieldName, IEnumerable<T> values, SPFieldType type)
            : base(operatorName, values, type)
        {
            FieldRef = new FieldRef() { Name = fieldName };
        }

        protected SingleFieldMultipleValueOperator(string operatorName, string fieldName, IEnumerable<Value<T>> values)
            : base(operatorName, values)
        {
            FieldRef = new FieldRef() { Name = fieldName };
        }

        protected SingleFieldMultipleValueOperator(string operatorName, FieldRef fieldRef, IEnumerable<T> values, SPFieldType type)
            : base(operatorName, values, type)
        {
            FieldRef = fieldRef;
        }

        protected SingleFieldMultipleValueOperator(string operatorName, FieldRef fieldRef, IEnumerable<Value<T>> values)
            : base(operatorName, values)
        {
            FieldRef = fieldRef;
        }

        protected SingleFieldMultipleValueOperator(string operatorName, string existingSingleFieldMultipleValueOperator)
            : base(operatorName, existingSingleFieldMultipleValueOperator)
        {
        }

        protected SingleFieldMultipleValueOperator(string operatorName, XElement existingSingleFieldMultipleValueOperator)
            : base(operatorName, existingSingleFieldMultipleValueOperator)
        {
        }

        protected override void OnParsing(XElement existingSingleFieldMultipleValueOperator)
        {
            XElement existingFieldRef = existingSingleFieldMultipleValueOperator.Elements().SingleOrDefault(el => string.Equals(el.Name.LocalName, "FieldRef", StringComparison.InvariantCultureIgnoreCase));

            if (existingFieldRef != null)
            {
                FieldRef = new FieldRef(existingFieldRef);
            }

            XElement existingValues = existingSingleFieldMultipleValueOperator.Elements().SingleOrDefault(el => string.Equals(el.Name.LocalName, "Values", StringComparison.InvariantCultureIgnoreCase));

            if (existingValues != null)
            {
                base.OnParsing(existingValues);
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
