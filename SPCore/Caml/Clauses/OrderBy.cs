using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SPCore.Caml.Clauses
{
    public sealed class OrderBy : Clause
    {
        public IEnumerable<FieldRef> FieldRefs { get; set; }

        public OrderBy(IEnumerable<FieldRef> fieldRefs)
            : base("OrderBy")
        {
            this.FieldRefs = fieldRefs;
        }

        public OrderBy(Guid fieldId, bool ascending)
            : base("OrderBy")
        {
            this.FieldRefs = new FieldRef[] { new FieldRef() { FieldId = fieldId, Ascending = ascending } };
        }

        public OrderBy(string fieldName, bool ascending)
            : base("OrderBy")
        {
            this.FieldRefs = new FieldRef[] { new FieldRef() { Name = fieldName, Ascending = ascending } };
        }

        public OrderBy(string fieldName)
            : this(fieldName, false)
        {
        }

        public OrderBy(Guid fieldId)
            : this(fieldId, false)
        {
        }

        public override XElement ToXElement()
        {
            var ele = base.ToXElement();

            foreach (var fieldRef in this.FieldRefs)
            {
                ele.Add(fieldRef.ToXElement());
            }

            return ele;
        }
    }
}
