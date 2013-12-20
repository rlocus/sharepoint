using System;
using System.Collections.Generic;
using System.Linq;
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
            FieldRefs = new FieldRef[] { new FieldRef() { FieldId = fieldId } };
            Collapse = collapse;
        }

        public GroupBy(string fieldName, bool collapse)
            : base("GroupBy")
        {
            FieldRefs = (new FieldRef[] { new FieldRef() { Name = fieldName } }).AsEnumerable();
            Collapse = collapse;
        }

        public GroupBy(string existingGroupBy)
            : base("GroupBy", existingGroupBy)
        {
        }

        public GroupBy(XElement existingGroupBy)
            : base("GroupBy", existingGroupBy)
        {
        }

        protected override void OnParsing(XElement existingGroupBy)
        {
            var existingFieldRefs = existingGroupBy.Elements().Where(el => string.Equals(el.Name.LocalName, "FieldRef", StringComparison.InvariantCultureIgnoreCase));
            FieldRefs = existingFieldRefs.Select(existingFieldRef => new FieldRef(existingFieldRef))/*.ToList()*/;

            XAttribute collaps = existingGroupBy.Attribute("Collapse");

            if (collaps != null)
            {
                Collapse = Convert.ToBoolean(collaps.Value);
            }
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

        public static GroupBy Combine(GroupBy firstGroupBy, GroupBy secondGroupBy)
        {
            GroupBy groupBy = null;
            bool collaps = false;

            var fieldRefs = new List<FieldRef>();

            if (firstGroupBy != null && firstGroupBy.FieldRefs != null)
            {
                collaps = firstGroupBy.Collapse;
                fieldRefs.AddRange(firstGroupBy.FieldRefs);
            }

            if (secondGroupBy != null && secondGroupBy.FieldRefs != null)
            {
                collaps = collaps | secondGroupBy.Collapse;
                fieldRefs.AddRange(secondGroupBy.FieldRefs);
            }

            if (fieldRefs.Count > 0)
            {
                groupBy = new GroupBy(fieldRefs, collaps);
            }

            return groupBy;
        }
    }
}
