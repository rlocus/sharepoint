using System;
using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;
using SPCore.Formula.Elements.Basic;

namespace SPCore.Formula.Elements.Conditional
{
    public class IsError : Element, IConditionalType, IValueType
    {
        [RequiredParameter]
        public IConditionalType Condition;

        protected override string Template
        {
            get 
            {
                return "ISERROR({Condition})";
            }
        }

        /// <summary>
        /// Used to create: ISERROR({Condition})
        /// </summary>
        public IsError(IConditionalType condition)
        {
            this.Condition = condition;
        }  
      
        /// <summary>
        /// Used to create: ISERROR({Condition})
        /// </summary>
        public IsError(System.Linq.Expressions.Expression<Func<string>> conditionalExpression)
        {
            this.Condition = new Expression(conditionalExpression);
        }
    }
}
