using SPCore.Search.Linq.Interfaces;
using System;
using System.Linq.Expressions;

namespace SPCore.Search.Linq
{
    // Base class for all binary analyzers
    internal abstract class BinaryExpressionBaseAnalyzer : BaseAnalyzer
    {
        protected IOperandBuilder OperandBuilder;

        protected BinaryExpressionBaseAnalyzer(IOperationResultBuilder operationResultBuilder,
            IOperandBuilder operandBuilder) :
            base(operationResultBuilder)
        {
            this.OperandBuilder = operandBuilder;
        }

        public override bool IsValid(LambdaExpression expr)
        {
            // body should be BinaryExpression
            if (!(expr.Body is BinaryExpression))
            {
                return false;
            }
            var body = expr.Body as BinaryExpression;

            if (this.IsExpressionWithStringBasedSyntax(body.Right))
            {
                // operands are of types - subclasses of BaseFieldType
                if (!this.IsValidExpressionWithStringBasedSyntax(body))
                {
                    return false;
                }
            }
            else
            {
                // operands are of native types like int, bool, DateTime
                if (!this.IsValidExpressionWithNativeSyntax(body))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidExpressionWithStringBasedSyntax(BinaryExpression body)
        {
            // left operand for string based syntax should be indexer call
            var leftExpression = body.Left;
            if (!this.IsValidLeftExpressionWithStringBasedSyntax(leftExpression))
            {
                return false;
            }

            // right expression should be constant, variable or method call
            var rightExpression = body.Right;
            if (!this.IsValidRightExpressionWithStringBasedSyntax(rightExpression))
            {
                return false;
            }
            return true;
        }

        protected bool IsValidLeftExpressionWithStringBasedSyntax(Expression leftExpression)
        {
            if (!(leftExpression is MethodCallExpression))
            {
                return false;
            }
            var leftOperand = leftExpression as MethodCallExpression;
            if (leftOperand.Method.Name != ReflectionHelper.IndexerMethodName)
            {
                return false;
            }

            if (leftOperand.Arguments.Count != 1)
            {
                return false;
            }

            // parameter of indexer can be constant, variable or method call
            var argumentExpression = leftOperand.Arguments[0];
            if (!this.IsValidEvaluableExpression(argumentExpression))
            {
                return false;
            }

            // type of argument expression should be string
            return (argumentExpression.Type == typeof(string));
        }

        private bool IsExpressionWithStringBasedSyntax(Expression rightExpression)
        {
            // it is for case when right expression is a method call to IncludeTimeValue method
            rightExpression = ExpressionsHelper.RemoveIncludeTimeValueMethodCallIfAny(rightExpression);

            return (rightExpression.NodeType == ExpressionType.Convert &&
                rightExpression.Type.IsSubclassOf(typeof(BaseFieldType)));
        }

        private bool IsValidExpressionWithNativeSyntax(BinaryExpression body)
        {
            // left operand should be unary expression (Convert of indexer - like (string)x["foo"])
            var leftExpression = body.Left;
            if (!this.IsValidLeftExpressionWithNativeSyntax(leftExpression))
            {
                return false;
            }

            // right expression can be constant, variable or method call
            var rightExpression = body.Right;
            if (!this.IsValidRightExpressionWithNativeSyntax(rightExpression))
            {
                return false;
            }
            return true;
        }

        protected bool IsValidLeftExpressionWithNativeSyntax(Expression leftExpression)
        {
            if (!(leftExpression is UnaryExpression))
            {
                return false;
            }

            var left = leftExpression as UnaryExpression;
            
            return left.NodeType == ExpressionType.Convert && this.IsValidLeftExpressionWithStringBasedSyntax(left.Operand);
        }

        // Right expression for native syntax should be constant, variable or method call
        protected bool IsValidRightExpressionWithNativeSyntax(Expression rightExpression)
        {
            return this.IsValidEvaluableExpression(rightExpression);
        }

        // Right expression for string based syntax should be constant, variable or method call
        protected bool IsValidRightExpressionWithStringBasedSyntax(Expression rightExpression)
        {
            // it is for case when right expression is a method call to IncludeTimeValue method
            rightExpression = ExpressionsHelper.RemoveIncludeTimeValueMethodCallIfAny(rightExpression);

            // 1st convertion is conversion to specific subclass of BaseFieldType
            if (!(rightExpression is UnaryExpression))
            {
                return false;
            }
            if (rightExpression.NodeType != ExpressionType.Convert)
            {
                return false;
            }
            if (!rightExpression.Type.IsSubclassOf(typeof(BaseFieldType)))
            {
                return false;
            }

            // 2nd convertion is conversion to BaseFieldType
            var operandExpression = ((UnaryExpression)rightExpression).Operand;
            if (!(operandExpression is UnaryExpression))
            {
                return false;
            }
            if (operandExpression.NodeType != ExpressionType.Convert)
            {
                return false;
            }
            if (operandExpression.Type != typeof(BaseFieldType))
            {
                return false;
            }

            var expr = ((UnaryExpression)operandExpression).Operand;

            // operand should be valid native expression
            if (!this.IsValidRightExpressionWithNativeSyntax(expr))
            {
                return false;
            }

            // type of casted expression should be string (althoug compiler will not
            // allow to cast to subclass of BaseFieldType from anything except string - because
            // BaseFieldType has explicit conversion operator only for string, we need to do this
            // because there is possibility to cast from BaseFieldType to any subclass)
            return (expr.Type == typeof (string));
        }

        // Some info from value operand can be required for properly initialization of field ref operand
        // (e.g. if value operand is lookup id we need to add LookupId="True" attribute to field ref operand)
        protected IOperand GetFieldRefOperand(LambdaExpression expr, IOperand valueOperand)
        {
            if (!this.IsValid(expr))
            {
                throw new NonSupportedExpressionException(expr);
            }
            var body = expr.Body as BinaryExpression;
            return this.OperandBuilder.CreateColumnOperand(body.Left, valueOperand);
        }

        protected IOperand getValueOperand(LambdaExpression expr)
        {
            if (!this.IsValid(expr))
            {
                throw new NonSupportedExpressionException(expr);
            }
            var body = expr.Body as BinaryExpression;
            if (this.IsValidRightExpressionWithNativeSyntax(body.Right))
            {
                return this.OperandBuilder.CreateValueOperandForNativeSyntax(body.Right);
            }
            if (this.IsValidRightExpressionWithStringBasedSyntax(body.Right))
            {
                return this.OperandBuilder.CreateValueOperandForStringBasedSyntax(body.Right);
            }
            throw new NonSupportedExpressionException(body.Right);
        }

        protected IOperation getOperation<T>(LambdaExpression expr,
            Func<IOperationResultBuilder, IOperand, IOperand, T> creator)
            where T : IOperation
        {
            if (!this.IsValid(expr))
            {
                throw new NonSupportedExpressionException(expr);
            }
            var valueOperand = this.getValueOperand(expr);
            var fieldRefOperand = this.GetFieldRefOperand(expr, valueOperand);
            return creator(this.OperationResultBuilder, fieldRefOperand, valueOperand);
        }
    }
}
