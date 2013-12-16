using System;
using System.Linq.Expressions;
using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;
using Expression = SPCore.Formula.Elements.Basic.Expression;

namespace SPCore.Formula.Elements.Conditional
{
    public class If : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IConditionalType Condition;

        [RequiredParameter]
        public IValueType CaseTrue;

        [RequiredParameter]
        public IValueType CaseFalse;

        protected override string Template
        {
            get 
            {
                return "IF({Condition}" + SectionSeparator + "{CaseTrue}" + SectionSeparator + "{CaseFalse})";
            }
        }

        /// <summary>
        /// Used to create: IF({Condition}, {CaseTrue}, {CaseFalse})
        /// </summary>
        public If(IConditionalType condition, IValueType caseTrue, IValueType caseFalse)
        {
            this.Condition = condition;
            this.CaseTrue = caseTrue;
            this.CaseFalse = caseFalse;
        }

        /// <summary>
        /// Used to create: IF({Condition}, {CaseTrue}, {CaseFalse})
        /// </summary>
        public If(Expression<Func<string>> conditionalExpression, IValueType caseTrue, IValueType caseFalse)
        {
            this.Condition = new Expression(conditionalExpression);
            this.CaseTrue = caseTrue;
            this.CaseFalse = caseFalse;
        }

        /// <summary>
        /// Used to create: IF({Condition}, {CaseTrue}, {CaseFalse})
        /// </summary>
        public If(Expression<Func<string>> conditionalExpression, Expression<Func<string>> expressionTrue, IValueType caseFalse)
        {
            this.Condition = new Expression(conditionalExpression);
            this.CaseTrue = new Expression(expressionTrue);
            this.CaseFalse = caseFalse;
        }

        /// <summary>
        /// Used to create: IF({Condition}, {CaseTrue}, {CaseFalse})
        /// </summary>
        public If(Expression<Func<string>> conditionalExpression, Expression<Func<string>> expressionTrue, Expression<Func<string>> expressionFalse)
        {
            this.Condition = new Expression(conditionalExpression);
            this.CaseTrue = new Expression(expressionTrue);
            this.CaseFalse = new Expression(expressionFalse);
        }   
    }
}
