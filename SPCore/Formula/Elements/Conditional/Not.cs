using System;
using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;
using SPCore.Formula.Elements.Basic;

namespace SPCore.Formula.Elements.Conditional
{
    public class Not : Element, IConditionalType, IValueType
    {
        [RequiredParameter]
        public IConditionalType Condition;

        protected override string Template
        {
            get 
            {
                return "NOT({Condition})";
            }
        }

        /// <summary>
        /// Used to create: NOT({Condition})
        /// </summary>
        public Not(IConditionalType condition)
        {
            this.Condition = condition;
        }   
     
        /// <summary>
        /// Used to create: NOT({Condition})
        /// </summary>
        public Not(System.Linq.Expressions.Expression<Func<string>> conditionalExpression)
        {
            this.Condition = new Expression(conditionalExpression);
        }
    }
}
