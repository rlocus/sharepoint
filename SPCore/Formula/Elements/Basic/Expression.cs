using System;
using System.Linq.Expressions;
using System.Text;
using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.Basic
{
    public class Expression : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public string Value;

        private readonly string _binaryOperator;
        private readonly string _comparisonOperator;
        private readonly string _unaryOperator;

        protected override string Template
        {
            get 
            {
                return "{Value}";
            }
        }

        /// <summary>
        /// Used to create: {Value[0]}{BinaryOperator}{Value[1]}...{Value[n]}
        /// </summary>
        public Expression(BinaryOperation binaryOperation, params IValueType[] value)
        {
            this._binaryOperator = Operation.GetBinaryOperator(binaryOperation);

            StringBuilder sb = new StringBuilder(512);

            for (int i = 0; i < value.Length; i++)
            {
                sb.Append(value[i].ToString());

                if ((i + 1) < value.Length)
                {
                    sb.Append(this._binaryOperator);
                }
            }

            this.Value = sb.ToString();
        }

        /// <summary>
        /// Used to create: {UnaryOperation}{Value}
        /// </summary>
        public Expression(UnaryOperation unaryOperation, IValueType value)
        {
            StringBuilder sb = new StringBuilder(512);

            this._unaryOperator = Operation.GetUnaryOperator(unaryOperation);
            
            sb.Append(this._unaryOperator);
            sb.Append(value.ToString());

            this.Value = sb.ToString();
        }

        /// <summary>
        /// Used to create: {ValueLeft}{ComparisonOperator}{ValueRight}
        /// </summary>
        public Expression(ComparisonOperation comparisonOperation, IValueType valueLeft, IValueType valueRight)
        {
            StringBuilder sb = new StringBuilder(512);

            this._comparisonOperator = Operation.GetComparisonOperator(comparisonOperation);

            sb.Append(valueLeft.ToString());
            sb.Append(this._comparisonOperator);
            sb.Append(valueRight.ToString());

            this.Value = sb.ToString();
        }

        /// <summary>
        /// Used to create: {Value}
        /// </summary>
        public Expression(IValueType value)
        {
            this.Value = value.ToString();
        }

        /// <summary>
        /// Used to create: {ValueLeft}{ComparisonOperator}{ValueRight}
        /// Used to create: {UnaryOperation}{Value}
        /// Used to create: {Value[0]}{BinaryOperator}{Value[1]}...{Value[n]}
        /// </summary>
        public Expression(Expression<Func<string>> operationExpression)
        {
            Func<string> deleg = operationExpression.Compile();
            this.Value = deleg();
        }
    }
}
