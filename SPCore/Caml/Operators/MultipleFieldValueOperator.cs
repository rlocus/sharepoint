using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public abstract class MultipleFieldValueOperator<T> : ValueOperator<T>
    {
        public IEnumerable<FieldRef> FieldRefs { get; set; }

        protected MultipleFieldValueOperator(string operatorName, IEnumerable<string> fieldNames, T value, SPFieldType type)
            : base(operatorName, value, type)
        {
            var fieldRefs = fieldNames.Select(fieldName => new FieldRef() {Name = fieldName}).ToList();
            FieldRefs = fieldRefs;
        }

        protected MultipleFieldValueOperator(string operatorName, IEnumerable<Guid> fieldIds, T value, SPFieldType type)
            : base(operatorName, value, type)
        {
            var fieldRefs = fieldIds.Select(fieldId => new FieldRef() {FieldId = fieldId}).ToList();
            FieldRefs = fieldRefs;
        }

        public override XElement ToXElement()
        {
            var ele = base.ToXElement();
            foreach (var fieldRef in FieldRefs)
            {
                ele.Add(fieldRef.ToXElement());
            }
            return ele;
        }
    }
}
