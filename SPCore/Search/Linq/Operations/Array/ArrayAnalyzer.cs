using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;

namespace SPCore.Search.Linq.Operations.Array
{
    internal class ArrayAnalyzer : BaseAnalyzer
    {
        private readonly IOperandBuilder _operandBuilder;

        public ArrayAnalyzer(IOperationResultBuilder operationResultBuilder,
            IOperandBuilder operandBuilder) :
            base(operationResultBuilder)
        {
            this._operandBuilder = operandBuilder;
        }

        public override bool IsValid(LambdaExpression expr)
        {
            var body = expr.Body as NewArrayExpression;
            if (body == null) return false;
            var counter = 0;
            body.Expressions.ToList().ForEach(ex =>
            {
                if (ex.NodeType == ExpressionType.TypeAs)
                {
                    var unary = ex as UnaryExpression;
                    if (unary == null ||
                        (unary.Type != typeof(SPSearch.Asc) && unary.Type != typeof(SPSearch.Desc)
                        && unary.Type != typeof(SPSearch.OrderDirection))) return;
                    ex = unary.Operand;
                }
                var methodCall = ex as MethodCallExpression;
                if (methodCall == null) return;
                if (methodCall.Method.Name != ReflectionHelper.IndexerMethodName) return;
                if (methodCall.Arguments.Count != 1) return;
                counter++;
            });
            return (body.Expressions.Count == counter);
        }

        public override IOperation GetOperation(LambdaExpression expr)
        {
            if (!IsValid(expr))
            {
                throw new NonSupportedExpressionException(expr);
            }
            var operands = GetColumnOperandsWithOrdering(expr);
            return new ArrayOperation(this.OperationResultBuilder, operands);
        }

        private IOperand[] GetColumnOperandsWithOrdering(LambdaExpression expr)
        {
            var operands = new List<IOperand>();
            ((NewArrayExpression)expr.Body).Expressions.ToList().ForEach(ex =>
            {
                var orderDirection = SPSearch.OrderDirection.Default;
                if (ex.NodeType == ExpressionType.TypeAs)
                {
                    orderDirection = SPSearch.OrderDirection.Convert(ex.Type);
                    ex = ((UnaryExpression)ex).Operand;
                }
                operands.Add(this._operandBuilder.CreateColumnOperandWithOrdering(ex, orderDirection));
            });
            return operands.ToArray();
        }
    }
}


