using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operands
{
    internal class DoubleValueOperand : ValueOperand<double>
    {
        public DoubleValueOperand(double value) :
            base(typeof(SPManagedDataType.Double), value)
        {
        }

        public DoubleValueOperand(string value) :
            base(typeof(SPManagedDataType.Double), 0)
        {
            if (!double.TryParse(value, out Value))
            {
                throw new InvalidValueForOperandTypeException(value, Type);
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override Expression ToExpression()
        {
            return Expression.Constant(Value);
        }
    }
}