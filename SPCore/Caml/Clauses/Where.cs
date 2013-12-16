using SPCore.Caml.Operators;

namespace SPCore.Caml.Clauses
{
    public sealed class Where : Clause
    {
        public Where(params Operator[] operators)
            : base("Where", operators)
        {
        }
    }
}
