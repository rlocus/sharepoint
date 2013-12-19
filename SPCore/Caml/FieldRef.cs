using System;
using System.Xml.Linq;

namespace SPCore.Caml
{
    public sealed class FieldRef : QueryElement
    {
        public Guid FieldId { get; set; }
        public string Name { get; set; }
        public bool? Ascending { get; set; }
        public bool? Nullable { get; set; }
        public bool? LookupId { get; set; }

        public FieldRef()
            : base("FieldRef")
        {
        }

        public FieldRef(string existingFieldRef)
            : base("FieldRef", existingFieldRef)
        {
        }

        public FieldRef(XElement existingFieldRef)
            : base("FieldRef", existingFieldRef)
        {
        }

        protected override void OnParsing(XElement existingFieldRef)
        {
            XAttribute name = existingFieldRef.Attribute("Name");

            if (name != null)
            {
                Name = name.Value;
            }

            XAttribute id = existingFieldRef.Attribute("ID");

            if (id != null)
            {
                string guidString = id.Value.Trim();

                if (guidString.Length > 0)
                {
                    FieldId = new Guid(guidString);
                }
            }

            XAttribute ascending = existingFieldRef.Attribute("Ascending");

            if (ascending != null)
            {
                Ascending = Convert.ToBoolean(ascending.Value);
            }

            XAttribute nullable = existingFieldRef.Attribute("Nullable");

            if (nullable != null)
            {
                Nullable = Convert.ToBoolean(nullable.Value);
            }

            XAttribute lookupId = existingFieldRef.Attribute("LookupId");

            if (lookupId != null)
            {
                LookupId = Convert.ToBoolean(lookupId.Value);
            }
        }

        public override XElement ToXElement()
        {
            XElement el = new XElement("FieldRef");

            if (FieldId != Guid.Empty)
            {
                el.Add(new XAttribute("ID", FieldId));
            }
            else if (!string.IsNullOrEmpty(Name))
            {
                el.Add(new XAttribute("Name", Name));
            }
            if (Ascending.HasValue)
            {
                el.Add(new XAttribute("Ascending", Ascending.Value));
            }
            if (Nullable.HasValue)
            {
                el.Add(new XAttribute("Nullable", Nullable.Value));
            }
            if (LookupId.HasValue)
            {
                el.Add(new XAttribute("LookupId", LookupId.Value));
            }
            return el;
        }

        public override string ToString()
        {
            return ToXElement().ToString();
        }
    }
}
