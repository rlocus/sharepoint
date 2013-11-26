using System;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operands
{
    internal class BooleanValueOperand : ValueOperand<bool>
    {
        public BooleanValueOperand(bool value) :
            base(typeof(SPManagedDataType.Boolean), value)
        {
        }

        public BooleanValueOperand(string value) :
            base(typeof(SPManagedDataType.Boolean), false)
        {
            if (!bool.TryParse(value, out Value))
            {
                // boolean operand can have 1 and 0 as parameter
                if (!this.TryConvertViaInteger(value, out Value))
                {
                    throw new InvalidValueForOperandTypeException(value, Type);
                }
            }
        }

        private bool TryConvertViaInteger(string value, out bool result)
        {
            result = false;

            try
            {
                int val = Convert.ToInt32(value);

                if (val != 0 && val != 1)
                {
                    return false;
                }

                result = Convert.ToBoolean(val);
            }
            catch
            {
                return false;
            }
            return true;
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


