using System.Collections.Generic;
using SPCore.Search.Linq.Interfaces;
using SPCore.Search.Linq.Operands;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SPCore.Search.Linq.Operations.Array
{
    internal class ArrayOperation : OperationBase
    {
        private readonly IOperand[] _columnOperands;

        public int OperandsCount
        {
            get { return this._columnOperands == null ? 0 : this._columnOperands.Length; }
        }

        public ArrayOperation(IOperationResultBuilder operationResultBuilder,
            params IOperand[] columnOperands) :
            base(operationResultBuilder)
        {
            this._columnOperands = columnOperands;
        }

        public override IOperationResult ToResult()
        {
            List<string> results = new List<string>();

            if (_columnOperands != null)
            {
                results.AddRange(_columnOperands.Select(columnOperand => columnOperand.ToString()));
            }

            return this.OperationResultBuilder.CreateResult(results.ToArray());
        }

        public override Expression ToExpression()
        {
            if (this._columnOperands == null)
            {
                throw new NullReferenceException("ColumnOperands");
            }
            if (this._columnOperands.Any(x => x == null))
            {
                throw new NullReferenceException("ColumnOperand");
            }
            if (this._columnOperands.Any(x => !(x is ColumnOperand || x is ColumnOperandWithOrdering)))
            {
                throw new ArrayOperationShouldContainOnlyFieldRefOperandsException();
            }

            // if there is only 1 column operand - return single expression (not array)
            if (this._columnOperands.Count() == 1)
            {
                return this._columnOperands.First().ToExpression();
            }

            return Expression.NewArrayInit(typeof(object), this._columnOperands.Select(o => o.ToExpression()));
        }
    }
}


