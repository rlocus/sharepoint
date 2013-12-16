using System;
using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;
using SPCore.Formula.Elements.Basic;

namespace SPCore.Formula.Elements.Conditional
{
    public class IsNumber : Element, IConditionalType, IValueType
    {
        [RequiredParameter]
        public IConditionalType Condition;

        protected override string Template
        {
            get 
            {
                return "ISNUMBER({Condition})";
            }
        }

        /// <summary>
        /// Used to create: ISNUMBER({Condition})
        /// </summary>
        public IsNumber(IConditionalType condition)
        {
            this.Condition = condition;
        }  
      
        /// <summary>
        /// Used to create: ISNUMBER({Condition})
        /// </summary>
        public IsNumber(System.Linq.Expressions.Expression<Func<string>> conditionalExpression)
        {
            this.Condition = new Expression(conditionalExpression);
        }
    }
}
