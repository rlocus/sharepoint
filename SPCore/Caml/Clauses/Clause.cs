using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SPCore.Caml.Operators;

namespace SPCore.Caml.Clauses
{
    public abstract class Clause : QueryElement
    {
        public IEnumerable<Operator> Operators { get; protected set; }

        protected Clause(string clauseName, params Operator[] operators)
            : base(clauseName)
        {
            Operators = operators;
        }

        public override XElement ToXElement()
        {
            XElement el = base.ToXElement();

            if (Operators != null)
            {
                foreach (Operator op in Operators.Where(op => op != null))
                {
                    el.Add(op.ToXElement());
                }
            }

            return el;
        }
    }
}
