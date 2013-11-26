using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;
using SPCore.Search.Linq.Operations.AndAlso;
using SPCore.Search.Linq.Operations.Array;
using SPCore.Search.Linq.Operations.BeginsWith;
using SPCore.Search.Linq.Operations.Contains;
using SPCore.Search.Linq.Operations.Eq;
using SPCore.Search.Linq.Operations.Geq;
using SPCore.Search.Linq.Operations.Gt;
using SPCore.Search.Linq.Operations.IsNotNull;
using SPCore.Search.Linq.Operations.IsNull;
using SPCore.Search.Linq.Operations.Leq;
using SPCore.Search.Linq.Operations.Lt;
using SPCore.Search.Linq.Operations.Neq;
using SPCore.Search.Linq.Operations.OrElse;

namespace SPCore.Search.Linq.Factories
{
    internal class AnalyzerFactory : IAnalyzerFactory
    {
        private readonly IOperandBuilder _operandBuilder;
        private readonly IOperationResultBuilder _operationResultBuilder;

        public AnalyzerFactory(IOperandBuilder operandBuilder, IOperationResultBuilder operationResultBuilder)
        {
            this._operandBuilder = operandBuilder;
            this._operationResultBuilder = operationResultBuilder;
        }

        public IAnalyzer Create(LambdaExpression expr)
        {
            ExpressionType exprType = expr.Body.NodeType;

            if (exprType == ExpressionType.AndAlso)
            {
                return new AndAlsoAnalyzer(this._operationResultBuilder, this);
            }
            if (exprType == ExpressionType.OrElse)
            {
                return new OrElseAnalyzer(this._operationResultBuilder, this);
            }
            if (exprType == ExpressionType.NewArrayInit)
            {
                return new ArrayAnalyzer(this._operationResultBuilder, this._operandBuilder);
            }
            if (exprType == ExpressionType.GreaterThanOrEqual)
            {
                return new GeqAnalyzer(this._operationResultBuilder, this._operandBuilder);
            }
            if (exprType == ExpressionType.GreaterThan)
            {
                return new GtAnalyzer(this._operationResultBuilder, this._operandBuilder);
            }
            if (exprType == ExpressionType.LessThanOrEqual)
            {
                return new LeqAnalyzer(this._operationResultBuilder, this._operandBuilder);
            }
            if (exprType == ExpressionType.LessThan)
            {
                return new LtAnalyzer(this._operationResultBuilder, this._operandBuilder);
            }

            // it is not enough to check ExpressionType for IsNull operation.
            // We need also to check that right operand is null
            IsNullAnalyzer isNullAnalyzer;
            if (this.IsNullExpression(expr, out isNullAnalyzer))
            {
                return isNullAnalyzer;
            }
            // note that it is important to have check on IsNull before check on ExpressionType.Equal.
            // Because x["foo"] == null is also ExpressionType.Equal, but it should be translated
            // into <IsNull> instead of <Eq>
            if (exprType == ExpressionType.Equal)
            {
                return new EqAnalyzer(this._operationResultBuilder, this._operandBuilder);
            }

            // it is not enough to check ExpressionType for IsNotNull operation.
            // We need also to check that right operand is null
            IsNotNullAnalyzer isNotNullAnalyzer;
            if (this.IsNotNullExpression(expr, out isNotNullAnalyzer))
            {
                return isNotNullAnalyzer;
            }
            // note that it is important to have check on IsNotNull before check on ExpressionType.NotEqual.
            // Because x["foo"] != null is also ExpressionType.NotEqual, but it should be translated
            // into <IsNotNull> instead of <Neq>
            if (exprType == ExpressionType.NotEqual)
            {
                return new NeqAnalyzer(this._operationResultBuilder, this._operandBuilder);
            }

            var beginsWithAnalyzer = new BeginsWithAnalyzer(_operationResultBuilder, _operandBuilder);
            if (beginsWithAnalyzer.IsValid(expr)) return beginsWithAnalyzer;

            var containsAnalyzer = new ContainsAnalyzer(_operationResultBuilder, _operandBuilder);
            if (containsAnalyzer.IsValid(expr)) return containsAnalyzer;

            throw new NonSupportedExpressionTypeException(exprType);
        }

        private bool IsNullExpression(LambdaExpression expr, out IsNullAnalyzer analyzer)
        {
            // the simplest way to check if this IsNotNull expression - is to reuse IsNotNullAnalyzer
            analyzer = new IsNullAnalyzer(this._operationResultBuilder, this._operandBuilder);
            return analyzer.IsValid(expr);
        }

        private bool IsNotNullExpression(LambdaExpression expr, out IsNotNullAnalyzer analyzer)
        {
            // the simplest way to check if this IsNotNull expression - is to reuse IsNotNullAnalyzer
            analyzer = new IsNotNullAnalyzer(this._operationResultBuilder, this._operandBuilder);
            return analyzer.IsValid(expr);
        }
    }
}
