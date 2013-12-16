using System.Collections.Generic;
using System.Xml.Linq;

namespace SPCore.Caml.Operators
{
    public abstract class NestedOperator : Operator
    {
        public IEnumerable<Operator> Operators { get; set; }

        protected NestedOperator(string operatorName, params Operator[] operators)
            : base(operatorName)
        {
            Operators = operators;
        }

        public override XElement ToXElement()
        {
            var ele = base.ToXElement();
            foreach (var op in Operators)
            {
                ele.Add(op.ToXElement());
            }
            return ele;
        }
    }
}
