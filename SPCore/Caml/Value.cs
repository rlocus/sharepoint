using System;
using System.Xml.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SPCore.Helper;

namespace SPCore.Caml
{
    public sealed class Value<T> : QueryElement
    {
        public T Val { get; set; }
        public SPFieldType Type { get; set; }
        public bool? IncludeTimeValue { get; set; }

        public Value(T value, SPFieldType type)
            : base("Value")
        {
            Val = value;
            Type = type;
        }

        public Value(string existingValue)
            : base("Value", existingValue)
        {
        }

        public Value(XElement existingValue)
            : base("Value", existingValue)
        {
        }

        protected override void OnParsing(XElement existingValue)
        {
            XAttribute type = existingValue.Attribute("Type");

            if (type != null)
            {
                Type = (SPFieldType)Enum.Parse(typeof(SPFieldType), type.Value.Trim(), true);
            }

            XAttribute includeTimeValue = existingValue.Attribute("IncludeTimeValue");

            if (includeTimeValue != null)
            {
                IncludeTimeValue = Convert.ToBoolean(includeTimeValue.Value);
            }

            if (!string.IsNullOrEmpty(existingValue.Value))
            {
                if (SPFieldType.DateTime == this.Type)
                {
                    if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(object))
                        Val = (T)((object)SPUtility.CreateDateTimeFromISO8601DateTimeString(existingValue.Value));
                }
                else
                {
                    //Val = (T)SPConverter.ConvertValue(SPConverter.GetValueType(Type), existingValue.Value);
                    Val = SPConverter.ConvertValue<T>(existingValue.Value);
                }
            }
        }

        public override XElement ToXElement()
        {
            XElement el = base.ToXElement();
            el.Add(new XAttribute("Type", Type));

            if (IncludeTimeValue.HasValue)
            {
                el.Add(new XAttribute("IncludeTimeValue", IncludeTimeValue.Value));
            }

            if (SPFieldType.DateTime == this.Type)
            {
                el.Value = typeof(T) == typeof(DateTime) || typeof(T) == typeof(object)
                    ? SPUtility.CreateISO8601DateTimeFromSystemDateTime(Convert.ToDateTime(Val))
                    : Convert.ToString(Val);
            }
            else
            {
                el.Value = Convert.ToString(Val);
            }

            return el;
        }
    }
}
