using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operands
{
    internal class TextValueOperand : ValueOperand<string>
    {
        public TextValueOperand(string value) :
            base(typeof(SPManagedDataType.Text), value)
        {
        }

        public override string ToString()
        {
            return string.Format("'{0}'", Value);
        }

        public override Expression ToExpression()
        {
            return Expression.Constant(this.Value);
        }
    }
}


