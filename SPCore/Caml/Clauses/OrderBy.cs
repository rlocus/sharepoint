using System;
using System.Collections.Generic;
using System.Linq;
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

        public OrderBy(XElement existingOrderBy)
            : base("OrderBy", existingOrderBy)
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

        protected override void OnParsing(XElement existingOrderBy)
        {
            var existingFieldRefs = existingOrderBy.Elements().Where(el => el.Name.LocalName == "FieldRef");
            FieldRefs = existingFieldRefs.Select(existingFieldRef => new FieldRef(existingFieldRef))/*.ToList()*/;
        }

        public override XElement ToXElement()
        {
            XElement el = base.ToXElement();

            if (this.FieldRefs != null)
            {
                foreach (FieldRef fieldRef in this.FieldRefs)
                {
                    if (fieldRef != null) el.Add(fieldRef.ToXElement());
                }
            }

            return el;
        }
    }
}
