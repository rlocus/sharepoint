using System;
using System.Linq.Expressions;

namespace SPCore.Search.Linq
{
    internal class GenericException : Exception
    {
        public GenericException(string message, params object[] args) :
            base(string.Format(message, args))
        {}        
    }

    internal class NonSupportedExpressionException : GenericException
    {
        public NonSupportedExpressionException(Expression expr) :
            base(ErrorMessages.NON_SUPPORTED_EXPRESSION, expr)
        {
        }
    }

    internal class NonSupportedExpressionTypeException : GenericException
    {
        public NonSupportedExpressionTypeException(ExpressionType exprType) :
            base(ErrorMessages.NON_SUPPORTED_EXPRESSION_TYPE, exprType)
        {
        }
    }

    internal class NonSupportedOperandTypeException : GenericException
    {
        public NonSupportedOperandTypeException(Type type) :
            base(ErrorMessages.NON_SUPPORTED_OPERAND_TYPE, type)
        {
        }
    }

    internal class NullValueOperandCannotBeTranslatedToQueryException : GenericException
    {
        public NullValueOperandCannotBeTranslatedToQueryException() :
            base(ErrorMessages.NULL_VALUE_OPERAND_CAN_NOT_BE_TRANSLATED_TO_SEARCH)
        {
        }
    }

    internal class InvalidValueForOperandTypeException : GenericException
    {
        public InvalidValueForOperandTypeException(object value, Type operandType) :
            base(ErrorMessages.INVALID_VALUE_FOR_OPERAND_TYPE, value, operandType)
        {
        }
    }

    //internal class DateTimeOperandModeNotSupportedException : GenericException
    //{
    //    public DateTimeOperandModeNotSupportedException(DateTimeValueOperand.DateTimeValueMode mode) :
    //        base(ErrorMessages.DATETIME_OPERAND_MODE_NOT_SUPPORTED, mode)
    //    {
    //    }
    //}

    internal class InvalidValueForFieldRefException : GenericException
    {
        public InvalidValueForFieldRefException(object value) :
            base(ErrorMessages.INVALID_VALUE_FOR_COLUMN_OPERAND, value)
        {
        }
    }

    internal class OnlyOrAndBinaryExpressionsAllowedForJoinsExceptions : GenericException
    {
        public OnlyOrAndBinaryExpressionsAllowedForJoinsExceptions() :
            base(ErrorMessages.ONLY_OR_AND_BINARY_EXPRESSIONS_ALLOWED_FOR_JOINS)
        {
        }
    }

    internal class EmptyExpressionsListException : GenericException
    {
        public EmptyExpressionsListException() :
            base(ErrorMessages.EMPTY_EXPRESSIONS_LIST)
        {
        }
    }

    internal class ColumnOperandShouldContainNameException : GenericException
    {
        public ColumnOperandShouldContainNameException() :
            base(ErrorMessages.EMPTY_EXPRESSIONS_LIST)
        {
        }
    }

    internal class ArrayOperationShouldContainOnlyFieldRefOperandsException : GenericException
    {
        public ArrayOperationShouldContainOnlyFieldRefOperandsException() :
            base(ErrorMessages.ARRAY_OPERATION_SHOULD_CONTAIN_ONLY_COLUMN_OPERANDS_EXCEPTION)
        {
        }
    }

    internal class OperationShouldContainFieldRefOperandException : GenericException
    {
        public OperationShouldContainFieldRefOperandException() :
            base(ErrorMessages.OPERATION_SHOULD_CONTAIN_COLUMN_OPERAND_EXCEPTION)
        {
        }
    }

    internal class OperationShouldContainTextValueOperandException : GenericException
    {
        public OperationShouldContainTextValueOperandException() :
            base(ErrorMessages.OPERATION_SHOULD_CONTAIN_TEXT_VALUE_OPERAND_EXCEPTION)
        {
        }
    }

    //internal class DateTimeValueOperandExpectedException : GenericException
    //{
    //    public DateTimeValueOperandExpectedException() :
    //        base(ErrorMessages.DATE_TIME_VALUE_OPERAND_EXPECTED_EXCEPTION)
    //    {
    //    }
    //}
    
    internal class IncorrectQueryException : Exception
    {
        public IncorrectQueryException(string tag)
            : base(string.Format("Query specified for tag '{0}' can not be translated to code", tag))
        { }
    }

    internal class CantParseBooleanAttributeException : Exception
    {
        public CantParseBooleanAttributeException(string attr) :
            base(string.Format("Can't parse boolean attribute '{0}'", attr))
        {
        }
    }

    internal class CantParseIntegerAttributeException : Exception
    {
        public CantParseIntegerAttributeException(string attr) :
            base(string.Format("Can't parse integer attribute '{0}'", attr))
        {
        }
    }

    internal class OnlyOnePartOfQueryShouldBeNotNullException : Exception
    {
        public OnlyOnePartOfQueryShouldBeNotNullException() :
            base(string.Format("Only one part of query can be not null: ({0}, {1}, {2}) or {3}",
            "Where", "OrderBy", "GroupBy", "ViewFields"))
        {
        }
    }
    
    internal class QueryAnalysisException : Exception
    {
        public QueryAnalysisException(string msg) :
            base(msg)
        {}
    }
}
