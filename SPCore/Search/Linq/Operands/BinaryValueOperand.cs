using System;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operands
{
    internal class BinaryValueOperand : ValueOperand<byte>
    {
        public BinaryValueOperand(byte value) :
            base(typeof(SPManagedDataType.Binary), value)
        {
        }

        public BinaryValueOperand(string value) :
            base(typeof(SPManagedDataType.Binary), 0)
        {
            if (!byte.TryParse(value, out Value))
            {
                throw new InvalidValueForOperandTypeException(value, Type);
            }
        }

        public override string ToString()
        {
            return Convert.ToInt32(Value).ToString();
        }

        public override Expression ToExpression()
        {
            return Expression.Constant(Value);
        }
    }
}


