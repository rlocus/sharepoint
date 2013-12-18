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

        public override XElement ToXElement()
        {
            var ele = base.ToXElement();
            if (Value != null) ele.Add(Value.ToXElement());
            return ele;
        }
    }
}
