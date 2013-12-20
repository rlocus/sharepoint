using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.SharePoint;
using SPCore.Caml.Interfaces;

namespace SPCore.Caml.Operators
{
    public abstract class MultipleValueOperator<T> : Operator, IMultipleValueOperator<T>
    {
        public IEnumerable<Value<T>> Values { get; set; }

        protected MultipleValueOperator(string operatorName, IEnumerable<T> values, SPFieldType type)
            : base(operatorName)
        {
            if (values != null) Values = values.Select(val => new Value<T>(val, type));
        }

        protected MultipleValueOperator(string operatorName, IEnumerable<Value<T>> values)
            : base(operatorName)
        {
            Values = values;
        }

        protected MultipleValueOperator(string operatorName, string existingMultipleValueOperator)
            : base(operatorName, existingMultipleValueOperator)
        {
        }

        protected MultipleValueOperator(string operatorName, XElement existingMultipleValueOperator)
            : base(operatorName, existingMultipleValueOperator)
        {
        }

        protected override void OnParsing(XElement existingValuesOperator)
        {
            var existingValues = existingValuesOperator.Elements().Where(el => string.Equals(el.Name.LocalName, "Value", StringComparison.InvariantCultureIgnoreCase));

            var values = new List<Value<T>>();

            foreach (XElement val in existingValues)
            {
                Value<T> value = new Value<T>(val);
                values.Add(value);
            }

            Values = values.AsEnumerable();
        }

        public override XElement ToXElement()
        {
            XElement el = base.ToXElement();

            if (Values != null)
            {
                el.Add(new XElement("Values", Values.Select(val => val != null ? val.ToXElement() : null)));
            }

            return el;
        }
    }
}
