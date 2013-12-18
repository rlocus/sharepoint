using System;
using System.Collections.Generic;
using SPCore.Caml.Interfaces;
using SPCore.Caml.Operators;

namespace SPCore.Caml.Clauses
{
    public sealed class Where : Clause
    {
        public Where(Operator op)
            : base("Where", new[] { op })
        {
            if (op == null) throw new ArgumentNullException("op");
        }

        public void And<T>(T op)
            where T : Operator, IFieldOperator
        {
            if (op == null) throw new ArgumentNullException("op");

            List<Operator> operators = new List<Operator>(Operators) { op };

            Operators = new[]
                            {
                                new And(operators.ToArray())
                            };
        }

        public void Or<T>(T op)
             where T : Operator, IFieldOperator
        {
            if (op == null) throw new ArgumentNullException("op");

            List<Operator> operators = new List<Operator>(Operators) { op };

            Operators = new[]
                            {
                                new Or(operators.ToArray())
                            };
        }
    }
}
