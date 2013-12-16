using System;
using System.Linq.Expressions;
using System.Text;
using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;
using Expression = SPCore.Formula.Elements.Basic.Expression;

namespace SPCore.Formula.Elements.Conditional
{
    public class And : Element, IConditionalType, IValueType
    {
        [RequiredParameter]
        public string Conditions;

        protected override string Template
        {
            get 
            {
                return "AND({Conditions})";
            }
        }

        /// <summary>
        /// Used to create: AND({Condition1},{Condition2}...)
        /// </summary>
        public And(params IConditionalType[] conditions)
        {
            StringBuilder sb = new StringBuilder(512);

            for (int i = 0; i < conditions.Length; i++)
            {
                sb.AppendFormat(conditions[i].ToString());

                if ((i + 1) < conditions.Length)
                {
                    sb.Append(SectionSeparator);
                }
            }

            this.Conditions = sb.ToString();
        }    
    
        /// <summary>
        /// Used to create: AND({Condition1}, {Condition2})
        /// </summary>
        public And(params Expression<Func<string>>[] conditionalExpressions)
        {
            StringBuilder sb = new StringBuilder(512);

            for (int i = 0; i < conditionalExpressions.Length; i++)
            {
                sb.AppendFormat(new Expression(conditionalExpressions[i]).ToString());

                if ((i + 1) < conditionalExpressions.Length)
                {
                    sb.Append(SectionSeparator);
                }
            }

            this.Conditions = sb.ToString();
        }
    }
}
