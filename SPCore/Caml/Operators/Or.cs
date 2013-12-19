
using System.Xml.Linq;

namespace SPCore.Caml.Operators
{
    public sealed class Or : NestedOperator
    {
        public Or(params Operator[] operators)
            : base("Or", operators)
        {
        }

        public Or(string existingOrOperator)
            : base("Or", existingOrOperator)
        {
        }

        public Or(XElement existingOrOperator)
            : base("Or", existingOrOperator)
        {
        }
    }
}
