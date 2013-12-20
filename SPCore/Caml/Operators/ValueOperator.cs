using System.Xml.Linq;
using Microsoft.SharePoint;
using SPCore.Caml.Interfaces;

namespace SPCore.Caml.Operators
{
    public abstract class ValueOperator<T> : Operator, IValueOperator<T>
    {
        public Value<T> Value { get; set; }

        protected ValueOperator(string operatorName, Value<T> value)
            : base(operatorName)
        {
            Value = value;
        }

        protected ValueOperator(string operatorName, T value, SPFieldType type)
            : base(operatorName)
        {
            Value = new Value<T>(value, type);
        }

        protected ValueOperator(string operatorName, string existingValueOperator)
            : base(operatorName, existingValueOperator)
        {
        }

        protected ValueOperator(string operatorName, XElement existingValueOperator)
            : base(operatorName, existingValueOperator)
        {
        }

        protected override void OnParsing(XElement existingValueOperator)
        {
            Value = new Value<T>(existingValueOperator);
        }

        public override XElement ToXElement()
        {
            XElement el = base.ToXElement();
            if (Value != null) el.Add(Value.ToXElement());
            return el;
        }
    }
}
