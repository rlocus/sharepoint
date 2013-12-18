using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SPCore.Caml.Clauses
{
    public sealed class GroupBy : Clause
    {
        public bool Collapse { get; set; }
        public IEnumerable<FieldRef> FieldRefs { get; set; }

        public GroupBy(IEnumerable<FieldRef> fieldRefs, bool collapse)
            : base("GroupBy")
        {
            FieldRefs = fieldRefs;
            Collapse = collapse;
        }

        public GroupBy(Guid fieldId, bool collapse)
            : base("GroupBy")
        {
            FieldRefs = new FieldRef[] { new FieldRef() { FieldId = fieldId, Ascending = false } };
            Collapse = collapse;
        }

        public GroupBy(string fieldName, bool collapse)
            : base("GroupBy")
        {
            FieldRefs = new FieldRef[] { new FieldRef() { Name = fieldName/*, Ascending = false*/ } };
            Collapse = collapse;
        }

        public override XElement ToXElement()
        {
            XElement el = base.ToXElement();
            el.Add(new XAttribute("Collapse", Collapse));

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
