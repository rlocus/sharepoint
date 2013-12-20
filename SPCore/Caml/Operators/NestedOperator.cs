using System.Collections.Generic;
using System.Linq;
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

        protected NestedOperator(string operatorName, string existingNestedOperator)
            : base(operatorName, existingNestedOperator)
        {
        }

        protected NestedOperator(string operatorName, XElement existingNestedOperator)
            : base(operatorName, existingNestedOperator)
        {
        }

        protected override void OnParsing(XElement existingNestedOperator)
        {
            List<Operator> operators = new List<Operator>();

            foreach (XElement element in existingNestedOperator.Elements())
            {
                var op = GetOperator(element);

                if (op != null)
                {
                   operators.Add(op);
                }
            }

            this.Operators = operators.AsEnumerable();
        }

        public override XElement ToXElement()
        {
            XElement el = base.ToXElement();

            foreach (Operator op in Operators.Where(op => op != null))
            {
                el.Add(op.ToXElement());
            }

            return el;
        }
    }
}
