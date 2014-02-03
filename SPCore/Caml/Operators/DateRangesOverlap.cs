using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public enum DateRangesOverlapValue
    {
        Now,
        Today,
        Day,
        Week,
        Month,
        Year
    }

    public sealed class DateRangesOverlap : MultipleFieldValueOperator<DateTime>
    {
        private DateRangesOverlapValue? _enumValue;

        public DateRangesOverlap(IEnumerable<FieldRef> fieldRefs, DateTime value)
            : base("DateRangesOverlap", fieldRefs, value, SPFieldType.DateTime)
        {
        }

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

        public DateRangesOverlap(string existingDateRangesOverlapOperator)
            : base("DateRangesOverlap", existingDateRangesOverlapOperator)
        {
        }

        public DateRangesOverlap(XElement existingDateRangesOverlapOperator)
            : base("DateRangesOverlap", existingDateRangesOverlapOperator)
        {
        }

        protected override void OnParsing(XElement existingMultipleFieldValueOperator)
        {
            base.OnParsing(existingMultipleFieldValueOperator);

            XElement existingValue = existingMultipleFieldValueOperator.Elements().SingleOrDefault(el => string.Equals(el.Name.LocalName, "Value", StringComparison.InvariantCultureIgnoreCase));

            if (existingValue != null && existingValue.HasElements)
            {
                DateRangesOverlapValue[] dateRangesOverlaps = new[]
                                                                  {
                                                                      DateRangesOverlapValue.Now,
                                                                      DateRangesOverlapValue.Today,
                                                                      DateRangesOverlapValue.Day,
                                                                      DateRangesOverlapValue.Week,
                                                                      DateRangesOverlapValue.Month,
                                                                      DateRangesOverlapValue.Year
                                                                  };

                foreach (XElement element in existingValue.Elements())
                {
                    if (dateRangesOverlaps.Any(dateRangesOverlap => dateRangesOverlap.ToString() == element.Name.LocalName))
                    {
                        _enumValue =
                            (DateRangesOverlapValue)Enum.Parse(typeof(DateRangesOverlapValue), element.Name.LocalName);
                        break;
                    }
                }
            }
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
