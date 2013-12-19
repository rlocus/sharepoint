using System.Xml.Linq;
using Microsoft.SharePoint;

namespace SPCore.Caml.Operators
{
    public abstract class ValueOperator<T> : Operator
    {
        public Value<T> Value { get; set; }

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
