using SPCore.Search.Linq.Interfaces;
using SPCore.Search.Linq.Operands;
using System;
using System.Linq.Expressions;

namespace SPCore.Search.Linq
{
    internal abstract class BinaryOperationBase : OperationBase
    {
        protected IOperand ColumnOperand;
        protected IOperand ValueOperand;

        protected BinaryOperationBase(IOperationResultBuilder operationResultBuilder,
            IOperand columnOperand, IOperand valueOperand) :
            base(operationResultBuilder)
        {
            this.ColumnOperand = columnOperand;
            this.ValueOperand = valueOperand;
        }

        // Here we know both FieldRef operand and Value operand. So in those cases when we need knowledge from FieldRef for Value,
        // or from Value to FieldRef (e.g. for native syntax we need to know the type of the Value operand in order to perform casting for FieldRef)
        protected virtual Expression GetColumnOperandExpression()
        {
            if (this.ColumnOperand == null)
            {
                throw new NullReferenceException("ColumnOperand");
            }
            if (this.ValueOperand == null)
            {
                throw new NullReferenceException("ValueOperand");
            }

            var columnOperandExpr = this.ColumnOperand.ToExpression();
            var valueOperandExpr = this.ValueOperand.ToExpression();
            var expr = valueOperandExpr as ConstantExpression;

            if (expr != null)
            {
                var valueOperandType = expr.Value.GetType();
                return Expression.Convert(columnOperandExpr, valueOperandType);
            }
            if (valueOperandExpr is NewExpression)
            {
                var valueOperandType = valueOperandExpr.Type;
                return Expression.Convert(columnOperandExpr, valueOperandType);
            }
            if (valueOperandExpr is MethodCallExpression)
            {
                // special case for DateTimeValueOperand - we should cast left value to the DateTime only if rvalue is native
                if (ValueOperand is DateTimeValueOperand /*&& ((DateTimeValueOperand)ValueOperand).Mode == DateTimeValueOperand.DateTimeValueMode.Native*/)
                {
                    var valueOperandType = ((MethodCallExpression) valueOperandExpr).Method.ReturnType;
                    return Expression.Convert(columnOperandExpr, valueOperandType);
                }
            }
            return columnOperandExpr;
        }

        protected virtual Expression GetValueOperandExpression()
        {
            if (this.ColumnOperand == null)
            {
                throw new NullReferenceException("ColumnOperand");
            }
            if (this.ValueOperand == null)
            {
                throw new NullReferenceException("ValueOperand");
            }

            return this.ValueOperand.ToExpression();
        }
    }
}
