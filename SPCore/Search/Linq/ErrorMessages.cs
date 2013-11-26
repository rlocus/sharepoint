namespace SPCore.Search.Linq
{
    internal static class ErrorMessages
    {
        public const string NON_SUPPORTED_EXPRESSION =  "Expression '{0}' can not be translated into Query";
        public const string NON_SUPPORTED_EXPRESSION_TYPE = "Expression type '{0}' is not supported";
        public const string NON_SUPPORTED_OPERAND_TYPE = "Operand type '{0}' is not supported";
        public const string NULL_VALUE_OPERAND_CAN_NOT_BE_TRANSLATED_TO_SEARCH =
            "Value is null. Null value is allowed only with '==' (IsNull) and '!=' (IsNotNull) operations. " +
            "Also null rvalue should not be casted to DataTypes.*";
        public const string INVALID_VALUE_FOR_OPERAND_TYPE = "Value '{0}' is not valid for operand type '{1}'";
        public const string INVALID_VALUE_FOR_COLUMN_OPERAND = "Value '{0}' is not valid column name for Column operand";
        public const string ONLY_OR_AND_BINARY_EXPRESSIONS_ALLOWED_FOR_JOINS = "Only 'OrElse' and 'AnsAlso' binary expressions are allowed for logical joins";
        public const string EMPTY_EXPRESSIONS_LIST = "Can not join list of expressions because it is empty. You should specify at least one expression in list";
        public const string FIELD_REF_SHOULD_CONTAIN_NAME = "Field ref element should contain at least one attribute: Name";
        public const string DATETIME_OPERAND_MODE_NOT_SUPPORTED = "Mode '{0}' is not supported for DateTime operand";
        public const string ARRAY_OPERATION_SHOULD_CONTAIN_ONLY_COLUMN_OPERANDS_EXCEPTION = "Array operation should contain only column operands";
        public const string OPERATION_SHOULD_CONTAIN_COLUMN_OPERAND_EXCEPTION = "Operation should contain Column Operand";
        public const string OPERATION_SHOULD_CONTAIN_TEXT_VALUE_OPERAND_EXCEPTION = "Operation should contain TextValueOperand";
    }
}
