using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint;
using SPCore.Caml.Interfaces;

namespace SPCore.Caml.Operators
{
    public abstract class MultipleFieldValueOperator<T> : ValueOperator<T>, IMultiFieldOperator
    {
        public IEnumerable<FieldRef> FieldRefs { get; set; }

        protected MultipleFieldValueOperator(string operatorName, IEnumerable<FieldRef> fieldRefs, T value, SPFieldType type)
            : base(operatorName, value, type)
        {
            FieldRefs = fieldRefs;
        }

        protected MultipleFieldValueOperator(string operatorName, IEnumerable<string> fieldNames, T value, SPFieldType type)
            : base(operatorName, value, type)
        {
            var fieldRefs = fieldNames.Select(fieldName => new FieldRef() { Name = fieldName })/*.ToList()*/;
            FieldRefs = fieldRefs;
        }

        protected MultipleFieldValueOperator(string operatorName, IEnumerable<Guid> fieldIds, T value, SPFieldType type)
            : base(operatorName, value, type)
        {
            var fieldRefs = fieldIds.Select(fieldId => new FieldRef() { FieldId = fieldId })/*.ToList()*/;
            FieldRefs = fieldRefs;
        }

        protected MultipleFieldValueOperator(string operatorName, string existingSingleFieldValueOperator)
            : base(operatorName, existingSingleFieldValueOperator)
        {
        }

        protected MultipleFieldValueOperator(string operatorName, XElement existingSingleFieldValueOperator)
            : base(operatorName, existingSingleFieldValueOperator)
        {
        }

        protected override void OnParsing(XElement existingMultipleFieldValueOperator)
        {
            var existingFieldRefs = existingMultipleFieldValueOperator.Elements().Where(el => string.Equals(el.Name.LocalName, "FieldRef", StringComparison.InvariantCultureIgnoreCase));
            FieldRefs = existingFieldRefs.Select(existingFieldRef => new FieldRef(existingFieldRef))/*.ToList()*/;
            XElement existingValue = existingMultipleFieldValueOperator.Elements().SingleOrDefault(el => string.Equals(el.Name.LocalName, "Value", StringComparison.InvariantCultureIgnoreCase));

            if (existingValue != null)
            {
                base.OnParsing(existingValue);
            }
        }

        public override XElement ToXElement()
        {
            XElement el = base.ToXElement();
            el.AddFirst(FieldRefs.Select(fieldRef => fieldRef != null ? fieldRef.ToXElement() : null));
            return el;
        }
    }
}
