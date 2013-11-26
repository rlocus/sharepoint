using System;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operands
{
    internal class GenericStringBasedValueOperand : ValueOperand<string>
    {
        public GenericStringBasedValueOperand(Type type, string value) : base(type, value)
        {
        }

        public override string ToString()
        {
            return Value;
        }

        public override Expression ToExpression()
        {
            return Expression.Convert(Expression.Convert(Expression.Constant(this.Value), typeof(BaseFieldType)), this.Type);
        }
    }
}
