using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operands
{
    internal class ColumnOperandWithOrdering : ColumnOperand
    {
        private readonly SPSearch.OrderDirection _orderDirection;

        public ColumnOperandWithOrdering(ColumnOperand columnOperand, SPSearch.OrderDirection orderDirection)
            : base(string.Empty) // just in order to avoid compiler error that base type doesn't contain parameterless constructor
        {
            if (!string.IsNullOrEmpty(columnOperand.ColumnName))
            {
                this.Initialize(columnOperand.ColumnName);
            }
            else
            {
                throw new ColumnOperandShouldContainNameException();
            }
            this._orderDirection = orderDirection;
        }

        public override string ToString()
        {
            return _orderDirection.IsDefault()
                       ? string.Format("{0}", base.ToString())
                       : string.Format("{0} {1}", base.ToString(), _orderDirection);
        }

        public override Expression ToExpression()
        {
            var expr = base.ToExpression();

            return this._orderDirection.IsDefault() ? expr : Expression.TypeAs(expr, this._orderDirection.GetType());
        }
    }
}
