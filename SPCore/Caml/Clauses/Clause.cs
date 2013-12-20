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
            if (operators != null) Operators = operators.AsEnumerable();
        }

        protected Clause(string clauseName, string existingClause)
            : base(clauseName, existingClause)
        {
        }

        protected Clause(string clauseName, XElement existingClause)
            : base(clauseName, existingClause)
        {
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
