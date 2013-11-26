using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operands
{
    internal class IntegerValueOperand : ValueOperand<int>
    {
        public IntegerValueOperand(int value) :
            base(typeof(SPManagedDataType.Integer), value)
        {
        }

        public IntegerValueOperand(string value) :
            base(typeof(SPManagedDataType.Integer), 0)
        {
            if (!int.TryParse(value, out Value))
            {
                throw new InvalidValueForOperandTypeException(value, this.Type);
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override Expression ToExpression()
        {
            return Expression.Constant(this.Value);
        }
    }
}


