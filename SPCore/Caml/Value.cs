using System;
using System.Xml.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SPCore.Caml
{
    public sealed class Value<T> : QueryElement
    {
        public T Val { get; set; }
        public SPFieldType Type { get; set; }

        public Value(T value, SPFieldType type)
            : base("Value")
        {
            Val = value;
            Type = type;
        }

        public override XElement ToXElement()
        {
            XElement el = base.ToXElement();
            el.Add(new XAttribute("Type", Type));

            if (SPFieldType.DateTime == this.Type)
            {
                if (typeof(T) == typeof(DateTime))
                {
                    el.Value = SPUtility.CreateISO8601DateTimeFromSystemDateTime(Convert.ToDateTime(Val));
                }
            }
            else
            {
                el.Value = Convert.ToString(Val);
            }

            return el;
        }
    }
}
