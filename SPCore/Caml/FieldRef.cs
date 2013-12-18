using System;
using System.Xml.Linq;

namespace SPCore.Caml
{
    public sealed class FieldRef : QueryElement
    {
        public Guid FieldId { get; set; }
        public string Name { get; set; }
        public bool? Ascending { get; set; }
        public bool? IncludeTimeValue { get; set; }
        public bool? Nullable { get; set; }

        public FieldRef()
            : base("FieldRef")
        {
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
            if (IncludeTimeValue.HasValue)
            {
                el.Add(new XAttribute("IncludeTimeValue", IncludeTimeValue.Value));
            }
            if (Nullable.HasValue)
            {
                el.Add(new XAttribute("Nullable", Nullable.Value));
            }
            return el;
        }
    }
}
