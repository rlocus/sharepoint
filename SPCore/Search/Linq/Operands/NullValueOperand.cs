using SPCore.Search.Linq.Interfaces;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operands
{
    // This is marker class which is used when passed value is null
    internal class NullValueOperand : IOperand
    {
        public override string ToString()
        {
            return string.Empty;
        }

        public Expression ToExpression()
        {
            return Expression.Constant(null);
        }

        public object GetValue()
        {
            return null;
        }
    }
}
