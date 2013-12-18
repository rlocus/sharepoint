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

        public OrderBy(string fieldName)
            : this(fieldName, null)
        {
        }

        public OrderBy(Guid fieldId)
            : this(fieldId, null)
        {
        }

        public OrderBy(Guid fieldId, bool? ascending)
            : base("OrderBy")
        {
            this.FieldRefs = new FieldRef[] { new FieldRef() { FieldId = fieldId, Ascending = ascending } };
        }

        public OrderBy(string fieldName, bool? ascending)
            : base("OrderBy")
        {
            this.FieldRefs = new FieldRef[] { new FieldRef() { Name = fieldName, Ascending = ascending } };
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();

            foreach (FieldRef fieldRef in this.FieldRefs)
            {
                el.Add(fieldRef.ToXElement());
            }

            return el;
        }
    }
}
