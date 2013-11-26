using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SPCore.Search.Linq.Interfaces;
using SPCore.Search.Linq.Operands;

namespace SPCore.Search.Linq.Factories
{
    internal class OperandBuilder : IOperandBuilder
    {
        // ----- Column Operand -----

        public IOperand CreateColumnOperand(Expression expr, IOperand valueOperand)
        {
            if (expr is UnaryExpression)
            {
                expr = ((UnaryExpression)expr).Operand;
            }
            var methodCallExpression = expr as MethodCallExpression;
            var argumentExpression = methodCallExpression.Arguments[0];
            if (argumentExpression is ConstantExpression)
            {
                return this.CreateColumnOperandFromConstantExpression(argumentExpression as ConstantExpression,
                    valueOperand);
            }
            return this.CreateColumnOperandFromNonConstantExpression(argumentExpression, valueOperand);
        }

        private IOperand CreateColumnOperandFromNonConstantExpression(Expression expr, IOperand valueOperand)
        {
            var value = EvaluateExpression(expr);
            
            if (value == null || (value.GetType() != typeof(string)))
            {
                throw new InvalidValueForFieldRefException(value);
            }

            return this.CreateColumnOperandByName((string)value, valueOperand);
        }

        private IOperand CreateColumnOperandFromConstantExpression(ConstantExpression expr, IOperand valueOperand)
        {
            // it is possible to create constant expression also of Guid type. See Query.ViewFields(IEnumerable<Guid> ids)
            //var val = expr.Value as string;
            var val = expr.Value != null ? expr.Value.ToString() : null;
            return this.CreateColumnOperandByName(val, valueOperand);
        }

        private IOperand CreateColumnOperandByName(string val, IOperand valueOperand)
        {
                return this.CreateColumnOperand(val, valueOperand);
        }

        private IOperand CreateColumnOperand(string fieldName, IOperand valueOperand)
        {
            var attrs = GetAdditionalAttributesForFieldRefOperands(valueOperand);
            return new ColumnOperand(fieldName, attrs);
        }

        private static List<KeyValuePair<string, string>> GetAdditionalAttributesForFieldRefOperands(IOperand valueOperand)
        {
            return new List<KeyValuePair<string, string>>();
        }

        public IOperand CreateColumnOperandWithOrdering(Expression expr, SPSearch.OrderDirection orderDirection)
        {
            var columnOperand = (ColumnOperand)CreateColumnOperand(expr, null);
            return new ColumnOperandWithOrdering(columnOperand, orderDirection);
        }

        // ----- Value Operand -----

        public IOperand CreateValueOperandForNativeSyntax(Expression expr)
        {
            // determine operand type from expression result (specify null as explicitOperandType)
            return CreateValueOperandForNativeSyntax(expr, null);
        }

        public IOperand CreateValueOperandForNativeSyntax(Expression expr, Type explicitOperandType)
        {
            return CreateValueOperandForNativeSyntax(expr, explicitOperandType, expr);
        }

        private IOperand CreateValueOperandForNativeSyntax(Expression expr, Type explicitOperandType, Expression sourceExpr)
        {
            if (expr is ConstantExpression)
            {
                return CreateValueOperandFromConstantExpression(expr as ConstantExpression, explicitOperandType, sourceExpr);
            }
            return this.CreateValueOperandFromNonConstantExpression(expr, explicitOperandType, sourceExpr);
        }

        public IOperand CreateValueOperandForStringBasedSyntax(Expression expr)
        {
            var newExpr = ExpressionsHelper.RemoveIncludeTimeValueMethodCallIfAny(expr);

            // retrieve internal native expression from string based syntax
            var internalExpression = ((UnaryExpression)((UnaryExpression)newExpr).Operand).Operand;

            // use conversion type as operand type (subclass of BaseFieldType should be used here)
            // because conversion operand has always string type for string based syntax
            return this.CreateValueOperandForNativeSyntax(internalExpression, newExpr.Type, expr);
        }

        private IOperand CreateValueOperandFromNonConstantExpression(Expression expr, Type explicitOperandType, Expression sourceExpr)
        {
            object value = EvaluateExpression(expr);

            // if operand type is not specified explicitly try to determine operand type from expression result
            var operandType = explicitOperandType;
            if (operandType == null)
            {
                // value can be null
                operandType = value != null ? value.GetType() : null;
            }
            return CreateValueOperand(operandType, value, sourceExpr);
        }

        private static object EvaluateExpression(Expression expr)
        {
            // need to add Expression.Convert(..) in order to define Func<object>
            var lambda = Expression.Lambda<Func<object>>(Expression.Convert(expr, typeof(object)));
            return lambda.Compile().Invoke();
        }

        private static IOperand CreateValueOperandFromConstantExpression(ConstantExpression expr, Type explicitOperandType, Expression sourceExpr)
        {
            // if operand type is not specified explicitly try to determine operand type from expression type
            var operandType = explicitOperandType ?? expr.Type;
            return CreateValueOperand(operandType, expr.Value, sourceExpr);
        }

        private static IOperand CreateValueOperand(Type type, object value, Expression expr)
        {
            bool includeTimeValue = ExpressionsHelper.IncludeTimeValue(expr);
            return CreateValueOperand(type, value, includeTimeValue, false, false);
        }

        internal static IOperand CreateValueOperand(Type type, object value, bool includeTimeValue, bool parseExactDateTime, bool isComparisionOperation)
        {
            // it is important to have check on NullValueOperand on 1st place
            if (value == null)
            {
                return new NullValueOperand();
            }
            // if cast to DataTypes.* class is required
            if (isComparisionOperation)
            {
                if (type.IsSubclassOf(typeof(BaseFieldTypeWithOperators)))
                {
                    return new GenericStringBasedValueOperand(type, (string)value);
                }
                // native operands are also supported. Several native operands are compirable
                if (type != typeof(DateTime) &&
                    type != typeof(int) &&
                    type != typeof(string) &&
                    type != typeof(decimal) &&
                    type != typeof(double) &&
                    type != typeof(byte))
                {
                    throw new NonSupportedOperandTypeException(type);
                }
            }
            // string operand can be native or string based
            if (type == typeof(string) || type == typeof(SPManagedDataType.Text))
            {
                return new TextValueOperand((string)value);
            }
            //number operand can be native or string based
            if (type == typeof(decimal) || type == typeof(SPManagedDataType.Decimal))
            {
                if (value.GetType() == typeof(decimal))
                    return new DecimalValueOperand((decimal)value);

                if (value.GetType() == typeof(string))
                    return new DecimalValueOperand((string)value);
            }

            if (type == typeof(double) || type == typeof(SPManagedDataType.Double))
            {
                if (value.GetType() == typeof(decimal))
                    return new DoubleValueOperand((double)value);

                if (value.GetType() == typeof(string))
                    return new DoubleValueOperand((string)value);
            }

            // integer operand can be native or string based
            if (type == typeof(int) || type == typeof(SPManagedDataType.Integer))
            {
                if (value.GetType() == typeof(int))
                {
                    return new IntegerValueOperand((int)value);
                }
                if (value.GetType() == typeof(string))
                {
                    return new IntegerValueOperand((string)value);
                }
            }
            // boolean operand can be native or string based
            if (type == typeof(bool) || type == typeof(SPManagedDataType.Boolean))
            {
                if (value.GetType() == typeof(bool))
                {
                    return new BooleanValueOperand((bool)value);
                }
                if (value.GetType() == typeof(string))
                {
                    return new BooleanValueOperand((string)value);
                }
            }

            if (type == typeof(byte) || type == typeof(SPManagedDataType.Binary))
            {
                if (value.GetType() == typeof(byte))
                    return new BinaryValueOperand((byte)value);

                if (value.GetType() == typeof(string))
                    return new BinaryValueOperand((string)value);
            }

            // DateTime operand can be native or string based
            if (type == typeof(DateTime) || type == typeof(SPManagedDataType.DateTime))
            {
                if (value.GetType() == typeof(DateTime))
                {
                    return new DateTimeValueOperand((DateTime)value, includeTimeValue);
                }
                if (value.GetType() == typeof(string))
                {
                    // for string based datetimes we need to specify additional parameter: should use ParseExact
                    // or simple Parse. Because from re it comes in sortable format ("s") and we need to use parse exact
                    return new DateTimeValueOperand((string)value, includeTimeValue, parseExactDateTime);
                }
            }
           
            throw new NonSupportedOperandTypeException(type);
        }
    }
}