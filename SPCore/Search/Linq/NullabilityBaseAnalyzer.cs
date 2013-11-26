using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;
using SPCore.Search.Linq.Operands;

namespace SPCore.Search.Linq
{
    internal abstract class NullabilityBaseAnalyzer : BinaryExpressionBaseAnalyzer
    {
        protected NullabilityBaseAnalyzer(IOperationResultBuilder operationResultBuilder, IOperandBuilder operandBuilder)
            : base(operationResultBuilder, operandBuilder)
        {
        }

        public override bool IsValid(LambdaExpression expr)
        {
            // do not call base.IsValid() here as convert is not required for IsNull/IsNotNull operations
            // (i.e. x["foo"] == null, instead of (T)x["foo"] == null). Convert on lvalue is optional here

            // body should be BinaryExpression
            if (!(expr.Body is BinaryExpression))
            {
                return false;
            }
            var body = expr.Body as BinaryExpression;

            // check right expression first here

            // if right expression has string based syntax we should not evaluate
            // it for IsNull or IsNotNull
            var rightExpression = body.Right;
            if (this.IsValidRightExpressionWithStringBasedSyntax(rightExpression))
            {
                return false;
            }

            if (!this.IsValidRightExpressionWithNativeSyntax(rightExpression))
            {
                return false;
            }

            // IsNull/IsNotNull expression may have and may not have convert on lvalue
            // (i.e. x["foo"] == null, instead of (T)x["foo"] == null). Convert on lvalue is optional here.
            // So both syntaxes are valid for left expression here: string based (without convert) and
            // native (with convert)
            if (!this.IsValidLeftExpressionWithStringBasedSyntax(body.Left) &&
                !this.IsValidLeftExpressionWithNativeSyntax(body.Left))
            {
                return false;
            }

            // check that right operand is null
            var valueOperand = this.OperandBuilder.CreateValueOperandForNativeSyntax(rightExpression);
            return (valueOperand is NullValueOperand);
        }
    }
}


