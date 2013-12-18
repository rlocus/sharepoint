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
            XElement el = base.ToXElement();
            
            foreach (Operator op in Operators)
            {
                el.Add(op.ToXElement());
            }

            return el;
        }
    }
}
