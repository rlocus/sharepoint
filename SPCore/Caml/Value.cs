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
            var ele = base.ToXElement();
            ele.Add(new XAttribute("Type", Type));

            if (SPFieldType.DateTime == this.Type)
            {
                if (typeof(T) == typeof(DateTime))
                {
                    ele.Value = SPUtility.CreateISO8601DateTimeFromSystemDateTime(Convert.ToDateTime(Val));
                }
            }
            else
            {
                ele.Value = Val.ToString();
            }
            return ele;
        }
    }
}
