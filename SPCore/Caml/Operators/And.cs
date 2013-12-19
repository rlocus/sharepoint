
using System.Xml.Linq;

namespace SPCore.Caml.Operators
{
    public sealed class And : NestedOperator
    {
        public And(params Operator[] operators)
            : base("And", operators)
        {
        }

        public And(string existingAndOperator)
            : base("And", existingAndOperator)
        {
        }

        public And(XElement existingAndOperator)
            : base("And", existingAndOperator)
        {
        }
    }
}
