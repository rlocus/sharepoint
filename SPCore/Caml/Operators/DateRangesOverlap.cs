using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public enum DateRangesOverlapValue
    {
        Today,
        Day,
        Week,
        Month,
        Year
    }

    public sealed class DateRangesOverlap : MultipleFieldValueOperator<DateTime>
    {
        private DateRangesOverlapValue? _enumValue;

        public DateRangesOverlap(DateTime value, params Guid[] fieldIds)
            : base("DateRangesOverlap", fieldIds, value, SPFieldType.DateTime)
        {
        }

        public DateRangesOverlap(DateTime value, params string[] fieldNames)
            : base("DateRangesOverlap", fieldNames, value, SPFieldType.DateTime)
        {
        }

        public DateRangesOverlap(DateRangesOverlapValue value, params Guid[] fieldIds)
            : base("DateRangesOverlap", fieldIds, DateTime.MinValue, SPFieldType.DateTime)
        {
            _enumValue = value;
        }

        public DateRangesOverlap(DateRangesOverlapValue value, params string[] fieldNames)
            : base("DateRangesOverlap", fieldNames, DateTime.MinValue, SPFieldType.DateTime)
        {
            _enumValue = value;
        }

        public override XElement ToXElement()
        {
            XElement el = base.ToXElement();

            if (_enumValue.HasValue)
            {
                XElement value = el.Elements("Value").Single();
                value.Value = string.Empty;
                value.Add(new XElement(_enumValue.ToString()));
            }

            return el;
        }
    }
}
