using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operands
{
    internal class DecimalValueOperand : ValueOperand<decimal>
    {
        public DecimalValueOperand(decimal value) :
            base(typeof (SPManagedDataType.Decimal), value)
        {
        }

        public DecimalValueOperand(string value) :
            base(typeof(SPManagedDataType.Decimal), 0)
        {
            if (!decimal.TryParse(value, out Value))
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